using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BiomesCore.Patches.Caverns;
using BiomesCore.Reflections;
using HarmonyLib;

namespace BiomesCaverns.Patches.VFEI
{
	[HarmonyPatch]
	internal static class PawnArrivalModeWorker_GigalocustSwarm_Patch
	{
		private const string TypeName = "PawnArrivalModeWorker_GigalocustSwarm";
		private const string LambdaTypeId = "DisplayClass";
		private const string LambdaMethodId = "b__";

		static bool Prepare(MethodBase original)
		{
			return ModAssemblies.VanillaFactionsExpandedInsectoid() != null;
		}

		[HarmonyTargetMethods]
		static IEnumerable<MethodInfo> TargetMethods()
		{
			var assembly = ModAssemblies.VanillaFactionsExpandedInsectoid();
			foreach (var type in assembly.GetTypes())
			{
				var typeName = type.FullName;
				if (typeName.Contains(TypeName))
				{
					// Both of the lambdas defined in different methods of PawnArrivalModeWorker_GigalocustSwarm must be patched.
					if (typeName.Contains(LambdaTypeId))
					{
						foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
						                                       BindingFlags.Instance | BindingFlags.Static))
						{
							if (method.Name.Contains(LambdaMethodId))
							{
								yield return method;
							}
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