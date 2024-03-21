using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Thrumbo incident should not trigger in cavern maps.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_ThrumboPasses), "CanFireNowSub")]
	public class IncidentWorker_ThrumboPasses_CanFireNowSub_Patch
	{
		private static void Postfix(ref bool __result, IncidentParms parms)
		{
			if (__result && parms.target is Map map)
			{
				var extension = map.Biome.GetModExtension<BiomesMap>();
				__result = extension == null || !extension.isCavern;
			}
		}
	}
}