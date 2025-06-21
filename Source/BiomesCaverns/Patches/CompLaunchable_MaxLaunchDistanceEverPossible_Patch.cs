using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Inject drillpod specific behavior in CompLaunchable.
	/// </summary>
	[HarmonyPatch(typeof(CompLaunchable), "MaxLaunchDistanceEver")]
	public class CompLaunchable_MaxLaunchDistanceEver_Patch
	{
		private static bool Prefix(CompLaunchable __instance, PlanetLayer layer, ref int __result)
		{
			if (__instance.parent.def != BC_DefOf.BMT_DrillPod)
			{
				return true;
			}

			int num = __instance.MaxLaunchDistanceEver(layer);

			__result = Mathf.FloorToInt(num / (2.25f * 3f));
			return false;
		}
	}
}