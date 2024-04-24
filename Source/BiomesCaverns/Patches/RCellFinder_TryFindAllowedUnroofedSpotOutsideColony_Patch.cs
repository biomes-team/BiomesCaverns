using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Although this function is generic, its only current vanilla usage is deciding where children can skydream.
	/// 
	/// </summary>
	[HarmonyPatch]
	public static class RCellFinder_TryFindAllowedUnroofedSpotOutsideColony_Patch
	{
		private static IEnumerable<MethodBase> TargetMethods()
        {
			yield return typeof(RCellFinder).GetLocalFunc(nameof(RCellFinder.TryFindAllowedUnroofedSpotOutsideColony),
				localFunc: "CellValidator");
			if (ModsConfig.IsActive("divineDerivative.Romance"))
			{
                yield return AccessTools.Method(Type.GetType("BetterRomance.DateUtility, WayBetterRomance"), "WalkCellValidator", new Type[] { typeof(IntVec3), typeof(Map) });
			}
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellHasNonCavernRoof(instructions.ToList());
		}
	}
}