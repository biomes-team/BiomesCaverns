using RimWorld;

namespace BiomesCaverns
{
	public static class Util
	{
		public static bool IsCavernBiome(BiomeDef def)
		{
			return def == BC_DefOf.BMT_CrystalCaverns ||
			       def == BC_DefOf.BMT_FungalForest ||
			       def == BC_DefOf.BMT_EarthenDepths;
		}
	}
}