using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow sky light and color changes to affect cavern maps.
	/// Since all incidents affecting the sky are patched to not be triggered, this should only affect phosphoric lights.
	/// </summary>
	[HarmonyPatch(typeof(SectionLayer_LightingOverlay), nameof(SectionLayer_LightingOverlay.Regenerate))]
	public class SectionLayer_LightingOverlay_Regenerate_Patch
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return BiomesCore.Patches.Caverns.Transpilers.CellHasNonCavernRoof(instructions.ToList());
		}
	}
}