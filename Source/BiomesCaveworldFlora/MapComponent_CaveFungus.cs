using System.Collections.Generic;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class MapComponent_CaveFungus : MapComponent
	{
		public List<ThingDef_FruitingBody> cavePlantDefsInternal = null;

		public int randomSpawnPeriodInTicks = 0;

		public int nextRandomSpawnTick = 500;

        public List<ThingDef_FruitingBody> CavePlantDefs
		{
			get
			{
				if (cavePlantDefsInternal.NullOrEmpty())
				{
					cavePlantDefsInternal = new List<ThingDef_FruitingBody>();
					foreach (ThingDef plantDef in DefDatabase<ThingDef>.AllDefs)
					{
						if (plantDef.category == ThingCategory.Plant)
						{
							ThingDef_FruitingBody FruitingBodyDef = plantDef as ThingDef_FruitingBody;
							if (FruitingBodyDef != null && (!FruitingBodyDef.growsOnlyInCaveBiome || map.Biome.defName == "Cave"))
							{
								cavePlantDefsInternal.Add(FruitingBodyDef);
                            }
						}
					}
				}
				return cavePlantDefsInternal;
			}
		}

		public MapComponent_CaveFungus(Map map)
			: base(map)
		{
        }

		public override void MapComponentTick()
		{
			if (randomSpawnPeriodInTicks == 0)
			{
				int mapSurfaceCoefficient = map.Size.x * 2 + map.Size.z * 2;
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
			ThingDef_FruitingBody cavePlantDef = CavePlantDefs.RandomElementByWeight((ThingDef_FruitingBody plantDef) => plantDef.MyceliumAbundance / plantDef.MyceliumSizeRange.Average);
			int newDesiredMyceliumSize = cavePlantDef.MyceliumSizeRange.RandomInRange;
			IntVec3 spawnCell = IntVec3.Invalid;
			GenCaveFungusReproduction.TryGetRandomMyceliumSpawnCell(cavePlantDef, newDesiredMyceliumSize, checkTemperature: true, map, out spawnCell);
			if (spawnCell.IsValid)
			{
				Mycelium.SpawnNewMyceliumAt(map, spawnCell, cavePlantDef, newDesiredMyceliumSize);
			}
		}

		public override void ExposeData()
		{
			Scribe_Values.Look(ref nextRandomSpawnTick, "nextRandomSpawnTick", 0);
		}
	}
}
