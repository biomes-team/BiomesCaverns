using HarmonyLib;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(TravellingTransporters), "TraveledPctStepPerTick", MethodType.Getter)]
	public class TravelingTransportPods_TraveledPctStepPerTick_Patch
	{
		[HarmonyPriority(Priority.Last)]
		private static void PostFix(TravellingTransporters __instance, ref float __result)
		{
			if (__instance.def == BC_DefOf.BMT_TravelingDrillPods && __result != 0f)
			{
                // Can't directly calculate this value anymore, modify it.
                // TODO what is the purpose of using (0.00025f / 10f) / num instead 
				// of vanilla's 0.00025f / num?
                float num = 0.00025f / __result;
				__result = (0.00025f / 10f) / num;
			}
		}
	}
}