using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Although this function is generic, its only current vanilla usage is deciding where children can skydream.
	/// 
	/// </summary>
	[HarmonyPatch]
	public static class RCellFinder_TryFindAllowedUnroofedSpotOutsideColony_Patch
	{
		private static MethodBase TargetMethod()
		{
			return typeof(RCellFinder).GetLocalFunc(nameof(RCellFinder.TryFindAllowedUnroofedSpotOutsideColony),
				localFunc: "CellValidator");
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return BiomesCore.Patches.Caverns.Transpilers.CellHasNonCavernRoof(instructions.ToList());
		}
	}
}