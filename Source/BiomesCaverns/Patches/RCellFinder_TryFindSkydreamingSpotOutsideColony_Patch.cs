using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch]
	public class RCellFinder_TryFindSkydreamingSpotOutsideColony_Patch
	{
		static MethodBase TargetMethod()
		{
			foreach (var nestedType in typeof(RCellFinder).GetNestedTypes(AccessTools.all))
			{
				foreach (var method in nestedType.GetMethods(AccessTools.all))
				{
					if (method.Name.Contains(nameof(RCellFinder.TryFindSkydreamingSpotOutsideColony)))
					{
						return method;
					}
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