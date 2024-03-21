using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches.VIEMS
{
	[HarmonyPatch]
	internal static class CompAbilityConstructANest_Apply_Patch
	{
		private const string TypeName = "CompAbilityConstructANest";
		private const string MethodName = "Apply";

		static bool Prepare(MethodBase original)
		{
			return ModAssemblies.VanillaIdeologyExpandedMemesStructures() != null;
		}

		static MethodBase TargetMethod()
		{
			var assembly = ModAssemblies.VanillaIdeologyExpandedMemesStructures();
			foreach (var type in assembly.GetTypes())
			{
				var typeName = type.FullName;
				if (!typeName.Contains(TypeName))
				{
					continue;
				}

				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
				                                       BindingFlags.Instance | BindingFlags.Static))
				{
					if (method.Name.Contains(MethodName))
					{
						return method;
					}
				}
			}

			return null;
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions.ToList(), Methods.GetRoofMethod,
				Methods.GetRoofThickIfCavernMethod);
		}
	}
}