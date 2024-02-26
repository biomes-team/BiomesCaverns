using RimWorld;
using Verse;

namespace BiomesCaverns.GenSteps
{
	class GenStep_ScatterStalagmite : GenStep_ScatterGroup
	{
		protected virtual bool ShouldSkipMapStalagmite(Map map)
		{
			BiomeDef biome = map.Biome;
			return biome != BC_DefOf.BMT_CrystalCaverns && biome != BC_DefOf.BMT_EarthenDepths &&
			       biome != BC_DefOf.BMT_FungalForest;
		}

		public override void Generate(Map map, GenStepParams parms)
		{
			if (ShouldSkipMapStalagmite(map) || ShouldSkipMap(map))
			{
				return;
			}
			int num = CalculateFinalCount(map);
			for (int i = 0; i < num; i++)
			{
				if (!TryFindScatterCell(map, out var result))
				{
					return;
				}

				ScatterAt(result, map, parms);
				usedSpots.Add(result);
			}

			usedSpots.Clear();
		}
	}
}