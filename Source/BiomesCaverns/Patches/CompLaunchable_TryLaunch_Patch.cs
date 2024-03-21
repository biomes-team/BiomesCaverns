using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(CompLaunchable), nameof(CompLaunchable.TryLaunch))]
	public class CompLaunchable_TryLaunch_Patch
	{
		private static ThingDef GetActiveDropPodDef(CompLaunchable instance)
		{
			return instance.parent.def == BC_DefOf.BMT_DrillPod ? BC_DefOf.BMT_DrillPodActive : ThingDefOf.ActiveDropPod;
		}

		private static WorldObjectDef GetWorldObjectDef(CompLaunchable instance)
		{
			return instance.parent.def == BC_DefOf.BMT_DrillPod
				? BC_DefOf.BMT_TravelingDrillPods
				: WorldObjectDefOf.TravelingTransportPods;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo ActiveDropPodField =
				AccessTools.Field(typeof(ThingDefOf), nameof(ThingDefOf.ActiveDropPod));
			MethodInfo GetActiveDropPodDefMethod =
				AccessTools.Method(typeof(CompLaunchable_TryLaunch_Patch), nameof(GetActiveDropPodDef));

			FieldInfo TravelingTransportPodsField =
				AccessTools.Field(typeof(WorldObjectDefOf), nameof(WorldObjectDefOf.TravelingTransportPods));
			MethodInfo GetWorldObjectDefMethod =
				AccessTools.Method(typeof(CompLaunchable_TryLaunch_Patch), nameof(GetWorldObjectDef));

			foreach (var code in instructions)
			{
				if (code.opcode == OpCodes.Ldsfld && code.operand as FieldInfo == ActiveDropPodField)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Call, GetActiveDropPodDefMethod);
				}
				else if (code.opcode == OpCodes.Ldsfld && code.operand as FieldInfo == TravelingTransportPodsField)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Call, GetWorldObjectDefMethod);
				}
				else
				{
					yield return code;
				}
			}
		}
	}
}