using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow drillpods to choose landing spots under cavern roof.
	/// </summary>
	[HarmonyPatch(typeof(DropCellFinder), nameof(DropCellFinder.RandomDropSpot))]
	public static class DropCellFinder_RandomDropSpot_Patch
	{
		public static bool Prefix(ref IntVec3 __result, Map map, bool standableOnly = true)
		{
			if (TravelingTransportPods_Arrived_Patch.drillPodIncoming)
			{
				__result = CellFinderLoose.RandomCellWith((IntVec3 c) => (!standableOnly || c.Standable(map)) && !c.Fogged(map),
					map);
				return false;
			}

			return true;
		}
	}
}