using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCaverns
{
    [HarmonyPatch(typeof(CompLaunchable), "MaxLaunchDistance", MethodType.Getter)]
    public class CompLaunchable_MaxLaunchDistance_Patch
    {
        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(CompLaunchable __instance, ref int __result)
        {
            if (__instance.parent.def == BC_DefOf.BMT_DrillPod)
            {
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
            return true;
        }
    }

    [HarmonyPatch(typeof(CompLaunchable), "MaxLaunchDistanceEverPossible", MethodType.Getter)]
    public class CompLaunchable_MaxLaunchDistanceEverPossible_Patch
    {
        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(CompLaunchable __instance, ref int __result)
        {
            if (__instance.parent.def == BC_DefOf.BMT_DrillPod)
            {
                if (!__instance.LoadingInProgressOrReadyToLaunch)
                {
                    __result = 0;
                    return false;
                }
                List<CompTransporter> transportersInGroup = __instance.TransportersInGroup;
                float num = 0f;
                for (int i = 0; i < transportersInGroup.Count; i++)
                {
                    Building fuelingPortSource = transportersInGroup[i].Launchable.FuelingPortSource;
                    if (fuelingPortSource != null)
                    {
                        num = Mathf.Max(num, fuelingPortSource.GetComp<CompRefuelable>().Props.fuelCapacity);
                    }
                }
                if (__instance.Props.fixedLaunchDistanceMax >= 0)
                {
                    __result = __instance.Props.fixedLaunchDistanceMax;
                    return false;
                }
                __result = Mathf.FloorToInt(num / (2.25f * 3f));
                return false;
            }
            return true;
        }
    }
}