using RimWorld.Planet;
using Verse;

namespace BiomesCaverns.FeatureWorkers
{
	public class FeatureWorker_CavernBiomeCluster : FeatureWorker_Cluster
	{
		protected override bool IsRoot(PlanetTile tile)
		{
			return def.rootBiomes.Contains(Find.WorldGrid[tile].PrimaryBiome);
		}

		protected override bool CanTraverse(PlanetTile tile, out bool ifRootThenRootGroupSizeMustMatch)
		{
			ifRootThenRootGroupSizeMustMatch = false;
			return Find.WorldGrid[tile].hilliness >= Hilliness.Mountainous;
		}
	}
}