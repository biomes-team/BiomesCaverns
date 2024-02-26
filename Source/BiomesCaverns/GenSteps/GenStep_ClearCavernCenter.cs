using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BiomesCaverns.GenSteps
{
	public abstract class GenStep_ClearCavernCenter : GenStep
	{
		public abstract int CenterRadius();

		public override void Generate(Map map, GenStepParams parms)
		{
			BiomeDef biome = map.Biome;
			if (biome != BC_DefOf.BMT_CrystalCaverns && biome != BC_DefOf.BMT_EarthenDepths &&
			    biome != BC_DefOf.BMT_FungalForest)
			{
				return;
			}

			foreach (var cell in GenRadial.RadialCellsAround(map.Center, CenterRadius(), true))
			{
				List<Thing> thingList = cell.GetThingList(map);
				for (int index = 0; index < thingList.Count; ++index)
				{
					var thing = thingList[index];
					var thingDef = thing.def;
					if (thingDef.IsBuildingArtificial || thingDef.building != null && thingDef.building.isNaturalRock)
					{
						thing.Destroy();
					}
				}
			}
		}

		public override int SeedPart => 626606246;
	}
}