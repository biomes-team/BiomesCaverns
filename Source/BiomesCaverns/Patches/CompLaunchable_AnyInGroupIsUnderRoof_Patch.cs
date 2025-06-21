using HarmonyLib;
using RimWorld;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow Drillpods to launch under roofs
	/// </summary>
	[HarmonyPatch(typeof(CompLaunchable), "AnyInGroupIsUnderRoof")]
	public static class CompLaunchable_AnyInGroupIsUnderRoof_Patch
	{
		public static void Postfix(CompLaunchable __instance, ref bool __result)
		{
			if (__instance.parent.def == BC_DefOf.BMT_DrillPod)
			{
				__result = false;
			}
		}
	}
}