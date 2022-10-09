using HarmonyLib;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BiomesCaverns
{
    [HarmonyPatch(typeof(TravelingTransportPods), "TraveledPctStepPerTick", MethodType.Getter)]
    public class TravelingTransportPods_TraveledPctStepPerTick_Patch
    {
        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(TravelingTransportPods __instance, ref float __result)
        {
            if (__instance.def == BC_DefOf.BMT_TravelingDrillPods)
            {
                Vector3 start = __instance.Start;
                Vector3 end = __instance.End;
                if (start == end)
                {
                    __result = 1f;
                    return false;
                }
                float num = GenMath.SphericalDistance(start.normalized, end.normalized);
                if (num == 0f)
                {
                    __result = 1f;
                    return false;
                }
                __result = (0.00025f / 10f) / num;
                return false;
            }
            return true;
        }
    }
}