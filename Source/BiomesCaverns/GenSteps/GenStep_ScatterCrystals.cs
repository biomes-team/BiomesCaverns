using Verse;

namespace BiomesCaverns.GenSteps
{
	public class GenStep_ScatterCrystals : GenStep_ScatterGroup
	{
		public override int SeedPart => -101767548;

		private static bool ShouldSkipMapCrystal(Map map)
		{
			return map.Biome != BC_DefOf.BMT_CrystalCaverns;
		}

		public override void Generate(Map map, GenStepParams parms)
		{
			if ( map.Biome == BC_DefOf.BMT_CrystalCaverns)
			{
				base.Generate(map, parms);
			}
		}
	}
}