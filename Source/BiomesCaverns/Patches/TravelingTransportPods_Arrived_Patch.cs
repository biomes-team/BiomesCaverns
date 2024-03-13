using HarmonyLib;
using RimWorld.Planet;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(TravelingTransportPods), "Arrived")]
	public static class TravelingTransportPods_Arrived_Patch
	{
		public static bool drillPodIncoming;

		public static void Prefix(TravelingTransportPods __instance)
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