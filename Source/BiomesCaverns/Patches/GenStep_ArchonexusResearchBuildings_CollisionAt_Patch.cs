using System.Collections.Generic;
using System.Linq;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow archonexus creation under cavern roof.
	/// </summary>
	[HarmonyPatch(typeof(GenStep_ArchonexusResearchBuildings), nameof(GenStep_ArchonexusResearchBuildings.CollisionAt))]
	internal static class GenStep_ArchonexusResearchBuildings_CollisionAt_Patch
	{
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions.ToList(),
				Methods.CellRoofedMethod, Methods.CellHasNonCavernRoofMethod);
		}
	}
}