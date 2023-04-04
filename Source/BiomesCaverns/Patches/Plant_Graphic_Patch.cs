using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(Plant), nameof(Plant.Graphic), MethodType.Getter)]
	internal class Plant_Graphic_Patch
	{
		internal static bool Prefix(ref Graphic __result, Plant __instance)
		{
			var extension = __instance.def.GetModExtension<DefModExtensions.PlantCavernGraphic>();
			if (extension != null && Util.IsCavernBiome(__instance.Map.Biome))
			{
				__result = extension.GetGraphic();
				return false;
			}

			return true;
		}
	}
}