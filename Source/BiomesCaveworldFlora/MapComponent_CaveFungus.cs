using System;
using System.Collections.Generic;
using System.Linq;
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
                    //Log.Error("0");
                    cavePlantDefsInternal = new List<ThingDef_FruitingBody>();
                    foreach (var plantDef in DefDatabase<ThingDef>.AllDefsListForReading)
                    {
                        if (plantDef.IsPlant)
                        {
                            if (plantDef is ThingDef_FruitingBody fruitingBodyDef && (!fruitingBodyDef.growsOnlyInCaveBiome || map.Biome.defName == "Cave"))
                            {
                                //Log.Error("1");
                                cavePlantDefsInternal.Add(fruitingBodyDef);
                            }
                        }
                    }
                }

                return cavePlantDefsInternal;
            }
        }

        public override void MapGenerated()
        {
            float baseGeneration = map.AllCells.ToList().Count * 0.02f;
            for (int i = 0; baseGeneration > i; i++)
            {
                TrySpawnNewMyceliumAtRandomPosition();
            }
        }

        public MapComponent_CaveFungus(Map map)
            : base(map) { }

        public override void MapComponentTick()
        {
            //Log.Error("2");
            if (randomSpawnPeriodInTicks == 0)
            {
                int mapSurfaceCoefficient = map.Size.x * 2 + map.Size.z * 2;
                // Prevent division by zero errors with mods using very small maps such as pocket dimensions.
                mapSurfaceCoefficient = Math.Max(mapSurfaceCoefficient, 1);
                int coeff = (mapSurfaceCoefficient / 100);
                randomSpawnPeriodInTicks = 160000 / (coeff > 0f ? coeff : 1);
            }

            if (Find.TickManager.TicksGame > nextRandomSpawnTick)
            {
                //Log.Error("3");
                nextRandomSpawnTick = Find.TickManager.TicksGame + randomSpawnPeriodInTicks;
                TrySpawnNewMyceliumAtRandomPosition();
            }
        }

        public void TrySpawnNewMyceliumAtRandomPosition()
        {
            //Log.Error(map.wildPlantSpawner.CachedChanceFromDensity.ToString());
            // Uncomment this if too much fungus spawn
            //if (!Rand.Chance(map.wildPlantSpawner.CachedChanceFromDensity))
            //{
            //    return;
            //}
            //Log.Error("4");
            var cavePlantDef = CavePlantDefs.RandomElementByWeight(plantDef => plantDef.MyceliumAbundance / plantDef.MyceliumSizeRange.Average);
            int newDesiredMyceliumSize = cavePlantDef.MyceliumSizeRange.RandomInRange;
            GenCaveFungusReproduction.TryGetRandomMyceliumSpawnCell(cavePlantDef, newDesiredMyceliumSize, checkTemperature: true, map, out var spawnCell);
            if (spawnCell.IsValid)
            {
                //Log.Error("5");
                Mycelium.SpawnNewMyceliumAt(map, spawnCell, cavePlantDef, newDesiredMyceliumSize);
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref nextRandomSpawnTick, "nextRandomSpawnTick");
        }
    }
}
