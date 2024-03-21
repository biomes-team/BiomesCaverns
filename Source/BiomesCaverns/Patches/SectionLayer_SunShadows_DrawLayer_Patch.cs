using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Disable sun shadows in cavern maps.
	/// </summary>
	[HarmonyPatch("SectionLayer_SunShadows", "DrawLayer")]
	public class SectionLayer_SunShadows_DrawLayer_Patch
	{
		public static bool Prefix(Section ___section)
		{
			BiomeDef biomeDef = ___section.map.Biome;
			return biomeDef == null || (biomeDef != BC_DefOf.BMT_CrystalCaverns && biomeDef != BC_DefOf.BMT_EarthenDepths &&
			                            biomeDef != BC_DefOf.BMT_FungalForest);
		}
	}
}