using Verse;

namespace BiomesCaverns.GenSteps
{
	class GenStep_ScatterCrystals : GenStep_ScatterGroup
	{
		private static bool ShouldSkipMapCrystal(Map map)
		{
			return map.Biome != BC_DefOf.BMT_CrystalCaverns;
		}

		public override void Generate(Map map, GenStepParams parms)
		{
			if (ShouldSkipMapCrystal(map) || ShouldSkipMap(map))
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