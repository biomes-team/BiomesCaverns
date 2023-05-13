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
	[HarmonyPatch(typeof(ShipLandingArea), nameof(ShipLandingArea.RecalculateBlockingThing))]
	public class ShipLandingArea_RecalculateBlockingThing_Patch
	{
		private static bool RoofedNotCavern(IntVec3 cell, Map map)
		{
			var roof = cell.GetRoof(map);
			return roof != null && roof != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo roofedNotCavernMethod =
				AccessTools.Method(typeof(ShipLandingArea_RecalculateBlockingThing_Patch), nameof(RoofedNotCavern));

			var instructionList = TranspilerHelper.ReplaceCall(instructions.ToList(),
				BiomesCore.Patches.Caverns.Methods.CellRoofedOriginal, roofedNotCavernMethod);

			foreach (var instruction in instructionList)
			{
				yield return instruction;
			}
		}
	}
}