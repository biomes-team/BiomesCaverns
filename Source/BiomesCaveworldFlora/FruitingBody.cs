using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class FruitingBody : Plant
    {
        public const float minGrowthToReproduce = 0.6f;

        private string cachedLabelMouseover = null;

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
                return base.Map.fertilityGrid.FertilityAt(base.Position);
            }
        }

        public bool IsFertilityConditionOk => FertilityGrowthRateFactor > 0f;

        public bool IsInCryostasis => base.Position.GetTemperature(base.Map) < (float)FruitingBodyProps.minGrowTemperature;

        public float TemperatureGrowthRateFactor
        {
            get
            {
                float temperature = base.Position.GetTemperature(base.Map);
                if (temperature < (float)FruitingBodyProps.minGrowTemperature || temperature > (float)FruitingBodyProps.maxGrowTemperature)
                {
                    return 0f;
                }
                if (temperature < (float)FruitingBodyProps.minOptimalGrowTemperature)
                {
                    return UnityEngine.Mathf.InverseLerp((float)FruitingBodyProps.minGrowTemperature, (float)FruitingBodyProps.minOptimalGrowTemperature, temperature);
                }
                if (temperature > (float)FruitingBodyProps.maxOptimalGrowTemperature)
                {
                    return UnityEngine.Mathf.InverseLerp((float)FruitingBodyProps.maxGrowTemperature, (float)FruitingBodyProps.maxOptimalGrowTemperature, temperature);
                }
                return 1f;
            }
        }

        public bool IsTemperatureConditionOk => TemperatureGrowthRateFactor > 0f;

        public float LightGrowthRateFactor
        {
            get
            {
                float light = base.Map.glowGrid.GameGlowAt(base.Position);
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
                Building edifice = base.Position.GetEdifice(base.Map);
                TerrainDef terrain = base.Position.GetTerrain(base.Map);
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
                        IntVec3 checkedPosition = base.Position + new IntVec3(xOffset, 0, zOffset);
                        if (checkedPosition.InBounds(base.Map))
                        {
                            Thing potentialRock = base.Map.thingGrid.ThingAt(checkedPosition, ThingCategory.Building);
                            if (potentialRock != null && potentialRock is Building && (potentialRock as Building).def.building.isNaturalRock)
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
                float temperature = base.Position.GetTemperature(base.Map);
                bool plantCanGrowHere = IsOnCaveFungusGrower || !Mycelium.DestroyedOrNull();
                if (temperature > (float)FruitingBodyProps.maxGrowTemperature || !IsLightConditionOk || !plantCanGrowHere)
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
                    StringBuilder stringBuilder = new StringBuilder();
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

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
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
                    base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things);
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
            StringBuilder stringBuilder = new StringBuilder();
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
                    if (base.Position.GetTemperature(base.Map) > (float)FruitingBodyProps.maxGrowTemperature)
                    {
                        stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.TooHot".Translate());
                    }
                    if (!IsLightConditionOk)
                    {
                        float light = base.Map.glowGrid.GameGlowAt(base.Position);
                        if (light < FruitingBodyProps.minLight)
                        {
                            stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.TooDark".Translate());
                        }
                        else if (light > FruitingBodyProps.maxLight)
                        {
                            stringBuilder.Append(", " + "Caveworld_Flora_Unleashed.Overlit".Translate());
                        }
                    }
                    if (FruitingBodyProps.growOnlyUndeRoof && !base.Map.roofGrid.Roofed(base.Position))
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
            return temperature >= (float)plantDef.minGrowTemperature && temperature <= (float)plantDef.maxGrowTemperature;
        }

        public static bool CanTerrainSupportPlantAt(ThingDef_FruitingBody plantDef, Map map, IntVec3 position)
        {
            bool isValidSpot = true;
            isValidSpot = ((!plantDef.growOnlyOnRoughRock) ? (isValidSpot & IsFertilityConditionOkAt(plantDef, map, position)) : (isValidSpot & IsNaturalRoughRockAt(map, position)));
            isValidSpot = ((!plantDef.growOnlyUndeRoof) ? (isValidSpot & !map.roofGrid.Roofed(position)) : (isValidSpot & map.roofGrid.Roofed(position)));
            if (plantDef.growOnlyNearNaturalRock)
            {
                isValidSpot &= IsNearNaturalRockBlock(map, position);
            }
            return isValidSpot;
        }

        public static bool IsNaturalRoughRockAt(Map map, IntVec3 position)
        {
            TerrainDef terrain = map.terrainGrid.TerrainAt(position);
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
                    IntVec3 checkedPosition = position + new IntVec3(xOffset, 0, zOffset);
                    if (checkedPosition.InBounds(map))
                    {
                        Thing potentialRock = map.thingGrid.ThingAt(checkedPosition, ThingCategory.Building);
                        if (potentialRock != null && potentialRock is Building && (potentialRock as Building).def.building.isNaturalRock)
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