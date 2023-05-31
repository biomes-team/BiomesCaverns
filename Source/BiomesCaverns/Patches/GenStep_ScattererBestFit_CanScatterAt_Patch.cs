using System.Collections.Generic;
using System.Linq;
using BiomesCore.Patches.Caverns;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(GenStep_ScattererBestFit), "CanScatterAt")]
	public class GenStep_ScattererBestFit_CanScatterAt_Patch
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions.ToList(),
				Methods.CellRoofedMethod, Methods.HasNonCavernRoofMethod);
		}
	}
}