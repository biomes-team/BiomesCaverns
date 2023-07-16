using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Patches.Caverns;
using BiomesCore.Reflections;
using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches.VPE_H
{
	[HarmonyPatch]
	public class Ability_SanguineSpear_Patches
	{
		private const string TypeName = "Ability_SanguineSpear";
		private static readonly string[] MethodNames = { "IsEnabledForPawn", "ValidateTarget" };

		static bool Prepare(MethodBase original)
		{
			return ModAssemblies.VanillaPsycastsExpandedHemosage() != null;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodInfo> TargetMethods()
		{
			var assembly = ModAssemblies.VanillaPsycastsExpandedHemosage();
			foreach (var type in assembly.GetTypes())
			{
				var currentTypeName = type.FullName;
				if (currentTypeName == null || !currentTypeName.Contains(TypeName))
				{
					continue;
				}

				foreach (var currentMethod in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
					         BindingFlags.Instance | BindingFlags.Static))
				{
					foreach (var targetMethod in MethodNames)
					{
						if (currentMethod.Name == targetMethod)
						{
							yield return currentMethod;
						}
					}
				}
			}
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return TranspilerHelper.ReplaceCall(instructions.ToList(),
				Methods.CellRoofedMethod, Methods.HasNonCavernRoofMethod);
		}
	}
}