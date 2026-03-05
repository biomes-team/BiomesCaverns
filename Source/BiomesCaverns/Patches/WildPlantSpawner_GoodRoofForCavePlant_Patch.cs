using BiomesCore;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(WildPlantSpawner), "GoodRoofForCavePlant")]
	public static class WildPlantSpawner_GoodRoofForCavePlant_Patch
	{

		public static bool Prefix(bool __result, Map ___map)
		{
			if (___map.IsCavern())
			{
				__result = false;
				return false;
			}
			return true;
		}

	}
}