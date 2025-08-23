using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches
{
    [HarmonyPatch(typeof(GenStep_Scatterer), "TryFindScatterCell")]
    public static class GenStep_Scatterer_TryFindScatterCell_Patch
    {
        public static void Prefix(Map map, GenStep_Scatterer __instance)
        {
            if (map.IsCavern())
            {
                __instance.allowRoofed = true;
            }
        }
    }

}