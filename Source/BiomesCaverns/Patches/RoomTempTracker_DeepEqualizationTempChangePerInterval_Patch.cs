using BiomesCaverns.ModSettings;
using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Disable enclosed thick roof area cooling feature from vanilla if the setting says so.
	/// </summary>
	[HarmonyPatch(typeof(RoomTempTracker), "DeepEqualizationTempChangePerInterval")]
	public static class RoomTempTracker_DeepEqualizationTempChangePerInterval_Patch
	{
		public static void Postfix(ref float __result)
		{
			if (!Settings.Values.CoolEnclosedThickRoofAreas)
			{
				__result = 0.0F;
			}
		}
	}
}