using System;
using System.Collections.Generic;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class MapComponent_CaveFungus : MapComponent
    {
        public List<ThingDef_FruitingBody> cavePlantDefsInternal;

        public int randomSpawnPeriodInTicks;

        public int nextRandomSpawnTick = 500;

        public List<ThingDef_FruitingBody> CavePlantDefs
        {
            get
            {
                if (cavePlantDefsInternal.NullOrEmpty())
                {
                    cavePlantDefsInternal = new List<ThingDef_FruitingBody>();
                    foreach (var plantDef in DefDatabase<ThingDef>.AllDefs)
                    {
                        if (plantDef.category == ThingCategory.Plant)
                        {
                            if (plantDef is ThingDef_FruitingBody fruitingBodyDef && (!fruitingBodyDef.growsOnlyInCaveBiome || map.Biome.defName == "Cave"))
                            {
                                cavePlantDefsInternal.Add(fruitingBodyDef);
                            }
                        }
                    }
                }

                return cavePlantDefsInternal;
            }
        }

        public MapComponent_CaveFungus(Map map)
            : base(map) { }

        public override void MapComponentTick()
        {
            if (randomSpawnPeriodInTicks == 0)
            {
                int mapSurfaceCoefficient = map.Size.x * 2 + map.Size.z * 2;
                // Prevent division by zero errors with mods using very small maps such as pocket dimensions.
                mapSurfaceCoefficient = Math.Max(mapSurfaceCoefficient, 1);
                randomSpawnPeriodInTicks = 160000 / (mapSurfaceCoefficient / 100);
            }

            if (Find.TickManager.TicksGame > nextRandomSpawnTick)
            {
                nextRandomSpawnTick = Find.TickManager.TicksGame + randomSpawnPeriodInTicks;
                TrySpawnNewMyceliumAtRandomPosition();
            }
        }

        public void TrySpawnNewMyceliumAtRandomPosition()
        {
            var cavePlantDef = CavePlantDefs.RandomElementByWeight(plantDef => plantDef.MyceliumAbundance / plantDef.MyceliumSizeRange.Average);
            int newDesiredMyceliumSize = cavePlantDef.MyceliumSizeRange.RandomInRange;
            GenCaveFungusReproduction.TryGetRandomMyceliumSpawnCell(cavePlantDef, newDesiredMyceliumSize, checkTemperature: true, map, out var spawnCell);
            if (spawnCell.IsValid)
            {
                Mycelium.SpawnNewMyceliumAt(map, spawnCell, cavePlantDef, newDesiredMyceliumSize);
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref nextRandomSpawnTick, "nextRandomSpawnTick");
        }
    }
}
