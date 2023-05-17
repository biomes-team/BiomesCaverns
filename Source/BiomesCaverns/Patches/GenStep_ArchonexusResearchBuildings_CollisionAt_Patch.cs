using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(GenStep_ArchonexusResearchBuildings), nameof(GenStep_ArchonexusResearchBuildings.CollisionAt))]
	public class GenStep_ArchonexusResearchBuildings_CollisionAt_Patch
	{
		public static bool RoofedNotCavern(IntVec3 cell, Map map)
		{
			var roof = cell.GetRoof(map);
			return roof == null || roof == BiomesCoreDefOf.BMT_RockRoofStable;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo cellRoofedMethod =
				AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.Roofed));

			MethodInfo roofedNotCavernMethod =
				AccessTools.Method(typeof(GenStep_ArchonexusResearchBuildings_CollisionAt_Patch), nameof(RoofedNotCavern));

			var instructionList = TranspilerHelper.ReplaceCall(instructions.ToList(),
				cellRoofedMethod, roofedNotCavernMethod);

			foreach (var instruction in instructionList)
			{
				yield return instruction;
			}
		}
	}
}