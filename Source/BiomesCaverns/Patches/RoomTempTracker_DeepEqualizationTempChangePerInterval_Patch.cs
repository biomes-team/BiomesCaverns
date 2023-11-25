using BiomesCaverns.ModSettings;
using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(RoomTempTracker), "DeepEqualizationTempChangePerInterval")]
	public static class RoomTempTracker_DeepEqualizationTempChangePerInterval_Patch
	{
		public static void Postfix(ref float __result, Room ___room)
		{
			if (___room.Map.Biome == BC_DefOf.BMT_EarthenDepths && !Settings.Values.AllowCoolingEnclosedEarthenDepthsAreas)
			{
				__result = 0.0F;
			}
		}
	}
}