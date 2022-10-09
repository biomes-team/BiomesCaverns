using HarmonyLib;
using RimWorld;

namespace BiomesCaverns
{
    [HarmonyPatch(typeof(CompLaunchable), "AnyInGroupIsUnderRoof", MethodType.Getter)]
    public static class CompLaunchable_AnyInGroupIsUnderRoof_Patch
    {
        public static void Postfix(CompLaunchable __instance, ref bool __result)
        {
            if (__instance.parent.def == BC_DefOf.BMT_DrillPod)
            {
                __result = false;
            }
        }
    }
}