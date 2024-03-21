using RimWorld;
using Verse;

namespace BiomesCaverns.GenSteps
{
	public class GenStep_ScatterStalagmite : GenStep_ScatterGroup
	{
		public override int SeedPart => 845823115;

		public override void Generate(Map map, GenStepParams parms)
		{
			BiomeDef biome = map.Biome;
			if (biome == BC_DefOf.BMT_CrystalCaverns || biome == BC_DefOf.BMT_EarthenDepths ||
			    biome == BC_DefOf.BMT_FungalForest)
			{
				base.Generate(map, parms);
			}
		}
	}
}