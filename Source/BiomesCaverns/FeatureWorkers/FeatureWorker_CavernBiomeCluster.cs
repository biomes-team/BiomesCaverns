using RimWorld.Planet;
using Verse;

namespace BiomesCaverns.FeatureWorkers
{
	public class FeatureWorker_CavernBiomeCluster : FeatureWorker_Cluster
	{
		protected override bool IsRoot(int tile)
		{
			return def.rootBiomes.Contains(Find.WorldGrid[tile].biome);
		}

		protected override bool CanTraverse(int tile, out bool ifRootThenRootGroupSizeMustMatch)
		{
			ifRootThenRootGroupSizeMustMatch = false;
			return Find.WorldGrid[tile].hilliness >= Hilliness.Mountainous;
		}
	}
}