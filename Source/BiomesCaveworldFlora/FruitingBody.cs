using System.Text;
using BiomesCore;
using RimWorld;
using UnityEngine;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class FruitingBody : Plant
    {
        public const float minGrowthToReproduce = 0.6f;

        private string cachedLabelMouseover;

        public Mycelium Mycelium;

        public ThingDef_FruitingBody FruitingBodyProps => def as ThingDef_FruitingBody;

        public float FertilityGrowthRateFactor
        {
            get
            {
                if (FruitingBodyProps.growOnlyOnRoughRock)
                {
                    return 1f;
                }

                return Map.fertilityGrid.FertilityAt(Position);
            }
        }

        public bool IsFertilityConditionOk => FertilityGrowthRateFactor > 0f;

        public bool IsInCryostasis => Position.GetTemperature(Map) < FruitingBodyProps.minGrowTemperature;

        public float TemperatureGrowthRateFactor
        {
            get
            {
                float temperature = Position.GetTemperature(Map);
                if (temperature < FruitingBodyProps.minGrowTemperature || temperature > FruitingBodyProps.maxGrowTemperature)
                {
                    return 0f;
                }

                if (temperature < FruitingBodyProps.minOptimalGrowTemperature)
                {
                    return Mathf.InverseLerp(FruitingBodyProps.minGrowTemperature, FruitingBodyProps.minOptimalGrowTemperature, temperature);
                }

                if (temperature > FruitingBodyProps.maxOptimalGrowTemperature)
                {
                    return Mathf.InverseLerp(FruitingBodyProps.maxGrowTemperature, FruitingBodyProps.maxOptimalGrowTemperature, temperature);
                }

                return 1f;
            }
        }

        public bool IsTemperatureConditionOk => TemperatureGrowthRateFactor > 0f;

        public float LightGrowthRateFactor
        {
            get
            {
                float light = Map.glowGrid.GameGlowAt(Position);
                if (light >= FruitingBodyProps.minLight && light <= FruitingBodyProps.maxLight)
                {
                    return 1f;
                }

                return 0f;
            }
        }

        public bool IsLightConditionOk => LightGrowthRateFactor > 0f;

        public new float GrowthRate => FertilityGrowthRateFactor * TemperatureGrowthRateFactor * LightGrowthRateFactor;

        public new float GrowthPerTick
        {
            get
            {
                if (LifeStage != PlantLifeStage.Growing || IsInCryostasis)
                {
                    return 0f;
                }

                float growthPerTick = 1f / (60000f * def.plant.growDays);
                return growthPerTick * GrowthRate;
            }
        }

        public bool IsOnCaveFungusGrower
        {
            get
            {
                var edifice = Position.GetEdifice(Map);
                var terrain = Position.GetTerrain(Map);
                if (terrain.defName.Contains("Fungal") || edifice != null && (edifice.def == Util_Caveworld_Flora_Unleashed.fungiponicsBasinDef || edifice.def == ThingDef.Named("PlantPot")))
                    return true;
                return false;
            }
        }

        public bool IsCellNearNaturalRockBlock
        {
            get
            {
                for (int xOffset = -2; xOffset <= 2; xOffset++)
                {
                    for (int zOffset = -2; zOffset <= 2; zOffset++)
                    {
                        var checkedPosition = Position + new IntVec3(xOffset, 0, zOffset);
                        if (checkedPosition.InBounds(Map))
                        {
                            var potentialRock = Map.thingGrid.ThingAt(checkedPosition, ThingCategory.Building);
                            if (potentialRock is Building building && building.def.building.isNaturalRock)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        public bool IsGrowingNow => LifeStage == PlantLifeStage.Growing && IsTemperatureConditionOk && IsLightConditionOk && (IsOnCaveFungusGrower || !Mycelium.DestroyedOrNull());

        public new bool Dying
        {
            get
            {
                if (IsInCryostasis)
                {
                    return false;
                }

                if (def.plant.LimitedLifespan && ageInt > def.plant.LifespanTicks)
                {
                    return true;
                }

                float temperature = Position.GetTemperature(Map);
                bool plantCanGrowHere = IsOnCaveFungusGrower || !Mycelium.DestroyedOrNull();
                if (temperature > FruitingBodyProps.maxGrowTemperature || !IsLightConditionOk || !plantCanGrowHere)
                {
                    return true;
                }

                return false;
            }
        }

        public override string LabelMouseover
        {
            get
            {
                if (cachedLabelMouseover == null)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append(def.LabelCap);
                    if (IsInCryostasis)
                    {
                        stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.Cryostasis".Translate());
                    }

                    if (Dying)
                    {
                        stringBuilder.Append(", " + "DyingLower".Translate());
                    }

                    stringBuilder.Append(")");
                    cachedLabelMouseover = stringBuilder.ToString();
                }

                return cachedLabelMouseover;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref Mycelium, "BMT_Mycelium");
        }

        public override void TickLong()
        {
            if (IsGrowingNow)
            {
                bool plantWasAlreadyMature = LifeStage == PlantLifeStage.Mature;
                growthInt += GrowthPerTick * 2000f;
                if (!plantWasAlreadyMature && LifeStage == PlantLifeStage.Mature)
                {
                    Map.mapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
                }
            }

            if (!IsInCryostasis)
            {
                if (LifeStage == PlantLifeStage.Mature)
                {
                    ageInt += 2000;
                }

                if (Dying)
                {
                    TakeDamage(new DamageInfo(DamageDefOf.Rotting, 5f));
                }
            }

            cachedLabelMouseover = null;
        }

        public override string GetInspectString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("PercentGrowth".Translate(GrowthPercentString));
            if (LifeStage == PlantLifeStage.Mature)
            {
                if (def.plant.Harvestable)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("ReadyToHarvest".Translate());
                }
                else
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Mature".Translate());
                }
            }
            else if (LifeStage == PlantLifeStage.Growing)
            {
                if (IsInCryostasis)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Caveworld_Flora_Unleashed.InCryostasis".Translate());
                }
                else if (Dying)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Caveworld_Flora_Unleashed.Dying".Translate());
                    if (Position.GetTemperature(Map) > FruitingBodyProps.maxGrowTemperature)
                    {
                        stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.TooHot".Translate());
                    }

                    if (!IsLightConditionOk)
                    {
                        float light = Map.glowGrid.GameGlowAt(Position);
                        if (light < FruitingBodyProps.minLight)
                        {
                            stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.TooDark".Translate());
                        }
                        else if (light > FruitingBodyProps.maxLight)
                        {
                            stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.Overlit".Translate());
                        }
                    }

                    if (FruitingBodyProps.growOnlyUndeRoof && !Map.roofGrid.Roofed(Position))
                    {
                        stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.Unroofed".Translate());
                    }
                    else if (!IsFertilityConditionOk)
                    {
                        stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.UnadaptedSoil".Translate());
                    }

                    if (FruitingBodyProps.growOnlyNearNaturalRock && !IsCellNearNaturalRockBlock)
                    {
                        stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.TooFarFromRock".Translate());
                    }

                    if (Mycelium.DestroyedOrNull())
                    {
                        stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.MyceliumRootRemoved".Translate());
                    }
                }
            }

            return stringBuilder.ToString();
        }

        public static bool IsFertilityConditionOkAt(ThingDef_FruitingBody plantDef, Map map, IntVec3 position)
        {
            float fertility = map.fertilityGrid.FertilityAt(position);
            return fertility >= plantDef.minFertility && fertility <= plantDef.maxFertility;
        }

        public static bool IsLightConditionOkAt(ThingDef_FruitingBody plantDef, Map map, IntVec3 position)
        {
            float light = map.glowGrid.GameGlowAt(position);
            if (light >= plantDef.minLight && light <= plantDef.maxLight)
            {
                return true;
            }

            return false;
        }

        public static bool IsTemperatureConditionOkAt(ThingDef_FruitingBody plantDef, Map map, IntVec3 position)
        {
            float temperature = position.GetTemperature(map);
            return temperature >= plantDef.minGrowTemperature && temperature <= plantDef.maxGrowTemperature;
        }

        public static bool CanTerrainSupportPlantAt(ThingDef_FruitingBody plantDef, Map map, IntVec3 position)
        {
            // Forbid spawns in closed rooms.
            var room = position.GetRoom(map);
            if (room != null && !room.PsychologicallyOutdoors)
            {
                return false;
            }

            // Forbid these fungi from spawning in deep cavern biomes.
            var roof = position.GetRoof(map);
            if (roof == BiomesCoreDefOf.BMT_RockRoofStable)
            {
                return false;
            }

            bool isValidSpot = true;
            isValidSpot = !plantDef.growOnlyOnRoughRock ? isValidSpot & IsFertilityConditionOkAt(plantDef, map, position) : isValidSpot & IsNaturalRoughRockAt(map, position);
            isValidSpot = !plantDef.growOnlyUndeRoof ? isValidSpot & !map.roofGrid.Roofed(position) : isValidSpot & map.roofGrid.Roofed(position);
            if (plantDef.growOnlyNearNaturalRock)
            {
                isValidSpot &= IsNearNaturalRockBlock(map, position);
            }

            return isValidSpot;
        }

        public static bool IsNaturalRoughRockAt(Map map, IntVec3 position)
        {
            var terrain = map.terrainGrid.TerrainAt(position);
            if (!terrain.layerable && terrain.defName.Contains("Rough"))
            {
                return true;
            }

            return false;
        }

        public static bool IsNearNaturalRockBlock(Map map, IntVec3 position)
        {
            for (int xOffset = -2; xOffset <= 2; xOffset++)
            {
                for (int zOffset = -2; zOffset <= 2; zOffset++)
                {
                    var checkedPosition = position + new IntVec3(xOffset, 0, zOffset);
                    if (checkedPosition.InBounds(map))
                    {
                        var potentialRock = map.thingGrid.ThingAt(checkedPosition, ThingCategory.Building);
                        if (potentialRock is Building building && building.def.building.isNaturalRock)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
