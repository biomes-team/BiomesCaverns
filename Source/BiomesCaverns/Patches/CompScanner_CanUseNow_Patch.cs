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
	[HarmonyPatch(typeof(CompScanner), nameof(CompScanner.CanUseNow), MethodType.Getter)]
	internal static class CompScanner_CanUseNow_Patch
	{
		/*
		static MethodBase TargetMethod()
		{
			foreach (var prop in typeof(CompScanner).GetProperties())
			{
				Log.Error(prop.ToString());
			}

			return typeof(CompScanner).GetProperty("CanUseNow");
		}
		*/

		private static bool IsAnyCellUnderNonCavernRoof(Thing thing)
		{
			CellRect cellRect = thing.OccupiedRect();
			bool flag = false;
			RoofGrid roofGrid = thing.Map.roofGrid;
			foreach (IntVec3 c in cellRect)
			{
				var roof = roofGrid.RoofAt(c);
				if (roof != null && roof != BiomesCoreDefOf.BMT_RockRoofStable)
				{
					flag = true;
					break;
				}
			}

			return flag;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo isAnyCellUnderRoofMethod =
				AccessTools.Method(typeof(RoofUtility), nameof(RoofUtility.IsAnyCellUnderRoof));

			MethodInfo isAnyCellUnderNonCavernRoofMethod =
				AccessTools.Method(typeof(CompScanner_CanUseNow_Patch), nameof(IsAnyCellUnderNonCavernRoof));

			var instructionList = TranspilerHelper.ReplaceCall(instructions.ToList(),
				isAnyCellUnderRoofMethod, isAnyCellUnderNonCavernRoofMethod);

			foreach (var instruction in instructionList)
			{
				yield return instruction;
			}
		}
	}
}