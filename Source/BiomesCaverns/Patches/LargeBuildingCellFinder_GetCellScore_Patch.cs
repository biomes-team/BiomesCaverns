using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(LargeBuildingCellFinder), "GetCellScore")]
	public class LargeBuildingCellFinder_GetCellScore_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellHasNonCavernRoof(instructions.ToList());
		}
	}
}