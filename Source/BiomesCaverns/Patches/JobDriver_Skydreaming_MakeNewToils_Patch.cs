using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Children can dream about the sky under cavern roof.
	/// </summary>
	[HarmonyPatch]
	internal static class JobDriver_Skydreaming_MakeNewToils_Patch
	{
		public static MethodBase TargetMethod()
		{
			return typeof(JobDriver_Skydreaming).GetLambda("MakeNewToils", lambdaOrdinal: 2);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return Transpilers.CellHasNonCavernRoof(instructions.ToList());
		}
	}
}