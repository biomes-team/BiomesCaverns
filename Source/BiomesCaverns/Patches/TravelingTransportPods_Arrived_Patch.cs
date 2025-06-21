using HarmonyLib;
using RimWorld.Planet;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow drillpods to choose landing spots under cavern roof.
	/// </summary>
	[HarmonyPatch(typeof(TravellingTransporters), "Arrived")]
	public static class TravelingTransportPods_Arrived_Patch
	{
		public static bool drillPodIncoming;

		public static void Prefix(TravellingTransporters __instance)
		{
			if (__instance.def == BC_DefOf.BMT_TravelingDrillPods)
			{
				drillPodIncoming = true;
			}
		}

		public static void Postfix()
		{
			drillPodIncoming = false;
		}
	}
}