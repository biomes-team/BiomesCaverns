using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch]
	public class RCellFinder_TryFindAllowedUnroofedSpotOutsideColony_Patch
	{
		static MethodBase TargetMethod()
		{
			foreach (MethodInfo method in AccessTools.GetDeclaredMethods(typeof(RCellFinder)))
			{
				if (method.Name.Contains("CellValidator"))
				{
					Log.Error("method");
					return method;
				}
			}

			return null;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return BiomesCore.Patches.Caverns.Transpilers.CellUnbreachableRoofed(instructions.ToList());
		}
	}
}