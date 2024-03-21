using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Inject drillpod specific behavior in CompLaunchable.
	/// </summary>
	[HarmonyPatch(typeof(CompLaunchable), nameof(CompLaunchable.MaxLaunchDistance), MethodType.Getter)]
	public class CompLaunchable_MaxLaunchDistance_Patch
	{
		private static bool Prefix(CompLaunchable __instance, ref int __result)
		{
			if (__instance.parent.def != BC_DefOf.BMT_DrillPod)
			{
				return true;
			}

			if (!__instance.LoadingInProgressOrReadyToLaunch)
			{
				__result = 0;
			}
			else if (__instance.Props.fixedLaunchDistanceMax >= 0)
			{
				__result = __instance.Props.fixedLaunchDistanceMax;
			}
			else
			{
				__result = Mathf.FloorToInt(__instance.FuelInLeastFueledFuelingPortSource / (2.25f * 3f));
			}

			return false;
		}
	}
}