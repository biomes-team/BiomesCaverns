using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch]
	public static class QuestPart_DropPods_GetRandomDropSpot_Patch
	{
		public static MethodBase TargetMethod()
		{
			return typeof(QuestPart_DropPods).GetLambda("GetRandomDropSpot");
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellHasNonCavernRoof(instructions.ToList());
		}
	}
}