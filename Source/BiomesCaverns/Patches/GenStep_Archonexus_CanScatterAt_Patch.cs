using System.Collections.Generic;
using System.Linq;
using BiomesCore.Patches.Caverns;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow archonexus creation under cavern roof.
	/// </summary>
	[HarmonyPatch(typeof(GenStep_Archonexus), "CanScatterAt")]
	internal static class GenStep_Archonexus_CanScatterAt_Patch
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions.ToList(),
				Methods.CellRoofedMethod, Methods.CellHasNonCavernRoofMethod);
		}
	}
}