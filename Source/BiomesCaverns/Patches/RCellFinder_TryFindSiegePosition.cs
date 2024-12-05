using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow sieges to choose spots under cavern roof.
	/// </summary>

	[HarmonyPatch(typeof(RCellFinder), "FindSiegePositionFrom")]
	public static class RCellFinder_FindSiegePositionFrom_Patch
	{
		public static bool Prefix(Map map, ref bool allowRoofed)
        {
            if (Utility.IsCavern(map))
            {
                allowRoofed = true;
            }
            return true;
        }
    }

	//[HarmonyPatch(typeof(RCellFinder), "TryFindSiegePosition")]
	//public static class RCellFinder_TryFindSiegePosition_Patch
	//{
	//	public static bool Prefix(Map map, ref bool allowRoofed)
	//	{
	//		if (Utility.IsCavern(map))
	//		{
	//			allowRoofed = true;
	//		}
	//		return true;
	//	}

	//}

}