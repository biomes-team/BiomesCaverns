using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{

    [HarmonyPatch(typeof(DropCellFinder), "IsGoodDropSpot")]
    public static class DropCellFinder_IsGoodDropSpot_Patch
    {
        public static void Prefix(Map map, ref bool canRoofPunch, ref bool allowIndoors)
        {
            if (map.IsCavern())
            {
                canRoofPunch = true;
                allowIndoors = true;
            }
        }
    }

}