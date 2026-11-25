using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Mech cluster positions are scored by how many of their cells are roofed or indoors. Every cell of a cavern has a
	/// cavern roof, so every candidate scored 0 and the first one found won, often inside the colony. Make the per-cell
	/// check cavern aware: a cavern roof does not count as roofed, and indoors uses the cavern aware room check.
	/// </summary>
	[HarmonyPatch]
	public static class MechClusterUtility_GetClusterPositionScore_Patch
	{
		private static MethodBase TargetMethod()
		{
			// CheckCell is a local function of GetClusterPositionScore.
			return AccessTools.FirstMethod(typeof(MechClusterUtility),
				method => method.Name.StartsWith("<GetClusterPositionScore>g__CheckCell"));
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CavernsAwarePsychologicallyOutdoors(Transpilers.CellHasNonCavernRoof(instructions.ToList()),
				OpCodes.Ldarg_0 // Cell.
			);
		}
	}
}
