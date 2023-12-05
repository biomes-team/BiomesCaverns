using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(ThingSetMaker_Meteorite), nameof(ThingSetMaker_Meteorite.Reset))]
	public static class ThingSetMaker_Meteorite_Reset_Patch
	{
		public static void Postfix(List<ThingDef> ___nonSmoothedMineables)
		{
			___nonSmoothedMineables.Remove(BC_DefOf.BMT_IceWall);
			___nonSmoothedMineables.Remove(BC_DefOf.BMT_SandWall);
		}
	}
}