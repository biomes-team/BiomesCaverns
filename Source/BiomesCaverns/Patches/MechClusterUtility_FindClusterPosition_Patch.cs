using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// When no candidate reaches a perfect score, vanilla FindClusterPosition remembers the random start cell of the
	/// best candidate instead of the siege position that was actually scored. Remember the scored position instead.
	/// Caverns rarely produce perfect scores, so this fallback is the common case there.
	/// </summary>
	[HarmonyPatch(typeof(MechClusterUtility), nameof(MechClusterUtility.FindClusterPosition))]
	public static class MechClusterUtility_FindClusterPosition_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codes = instructions.ToList();

			// IntVec3 result = IntVec3.Invalid;
			int resultInit = codes.FindIndex(code =>
				code.LoadsField(AccessTools.Field(typeof(IntVec3), nameof(IntVec3.Invalid))));
			int randomCellCall = codes.FindIndex(code =>
				code.Calls(AccessTools.Method(typeof(CellFinderLoose), nameof(CellFinderLoose.RandomCellWith))));
			int siegeCall = codes.FindIndex(code =>
				code.Calls(AccessTools.Method(typeof(RCellFinder), nameof(RCellFinder.FindSiegePositionFrom))));

			if (resultInit < 0 || randomCellCall < 0 || siegeCall < 0 || !codes[resultInit + 1].IsStloc() ||
			    !codes[randomCellCall + 1].IsStloc() || !codes[siegeCall + 1].IsStloc())
			{
				BiomesCore.BiomesCore.Error("MechClusterUtility_FindClusterPosition_Patch: IL pattern not found, patch skipped.");
				return codes;
			}

			int resultLocal = codes[resultInit + 1].LocalIndex();
			int startCellLocal = codes[randomCellCall + 1].LocalIndex();
			int siegeCellLocal = codes[siegeCall + 1].LocalIndex();

			// result = startCell; -> result = siegeCell;
			for (int i = siegeCall + 2; i < codes.Count - 1; i++)
			{
				if (codes[i].IsLdloc() && codes[i].LocalIndex() == startCellLocal && codes[i + 1].IsStloc() &&
				    codes[i + 1].LocalIndex() == resultLocal)
				{
					codes[i] = CodeInstruction.LoadLocal(siegeCellLocal).MoveLabelsFrom(codes[i]).MoveBlocksFrom(codes[i]);
					return codes;
				}
			}

			BiomesCore.BiomesCore.Error("MechClusterUtility_FindClusterPosition_Patch: result assignment not found, patch skipped.");
			return codes;
		}
	}
}
