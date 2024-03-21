using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Reflections;
using HarmonyLib;

namespace BiomesCaverns.Patches.VFEI
{
	/// <summary>
	/// Commented out patch, kept in case it is useful for future VFE - Insectoids 2 support.
	/// </summary>
	/*
	[HarmonyPatch]
	internal static class IncidentWorker_GiantInfestation_TryFindCell_Patch
	{
		private const string TypeName = "IncidentWorker_GiantInfestation";
		private const string LambdaTypeId = "DisplayClass";
		private const string LambdaMethodId = "b__";

		static bool Prepare(MethodBase original)
		{
			return false; //ModAssemblies.VanillaFactionsExpandedInsectoid() != null;
		}

		static MethodBase TargetMethod()
		{
			var assembly = ModAssemblies.VanillaFactionsExpandedInsectoid();
			foreach (var type in assembly.GetTypes())
			{
				var typeName = type.FullName;
				if (!typeName.Contains(TypeName))
				{
					continue;
				}

				if (typeName.Contains(LambdaTypeId))
				{
					foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
					                                       BindingFlags.Instance | BindingFlags.Static))
					{
						if (method.Name.Contains(LambdaMethodId))
						{
							return method;
						}
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
	*/
}