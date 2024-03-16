using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
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
			return BiomesCore.Patches.Caverns.Transpilers.CellUnbreachableRoofed(instructions.ToList());
		}
	}
}