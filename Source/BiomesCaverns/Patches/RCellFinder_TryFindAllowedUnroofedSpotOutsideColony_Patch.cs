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
				Log.Message("Trying to find WalkCellValidator");
				var type = Type.GetType("BetterRomance.DateUtility, WayBetterRomance");
				Log.Message($"BetterRomance.DateUtility {(type == null ? "not found" : "found")}");
				var method = AccessTools.Method(type, "WalkCellValidator", new Type[] { typeof(IntVec3), typeof(Map) });
                Log.Message($"WalkCellValidator {(method == null ? "not found" : "found")}");
                yield return method;
			}
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellHasNonCavernRoof(instructions.ToList());
		}
	}
}