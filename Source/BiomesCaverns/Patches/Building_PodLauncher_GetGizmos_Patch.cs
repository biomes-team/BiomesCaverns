using System.Collections.Generic;
using System.Linq;
using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// When playing on a cavern biome, the build drop pod gizmo is replaced with a build drill pod gizmo.
	/// </summary>
	[HarmonyPatch(typeof(Building_PodLauncher), nameof(Building_PodLauncher.GetGizmos))]
	internal static class Building_PodLauncher_GetGizmos_Patch
	{
		private static Gizmo DrillPodGizmo(Building_PodLauncher instance)
		{
			Command_Action action = null;
			var map = instance.Map;
			var def = BC_DefOf.BMT_DrillPod;

			var fuelingPortCell = FuelingPortUtility.GetFuelingPortCell(instance);
			var acceptanceReport = GenConstruct.CanPlaceBlueprintAt(def, fuelingPortCell, def.defaultPlacingRot, map);
			var designatorBuild = BuildCopyCommandUtility.FindAllowedDesignator(BC_DefOf.BMT_DrillPod);

			if (designatorBuild != null)
			{
				action = new Command_Action
				{
					defaultLabel = "BuildThing".Translate(BC_DefOf.BMT_DrillPod.label),
					icon = designatorBuild.icon,
					defaultDesc = designatorBuild.Desc,
					action = delegate
					{
						GenConstruct.PlaceBlueprintForBuild(BC_DefOf.BMT_DrillPod, fuelingPortCell, map,
							BC_DefOf.BMT_DrillPod.defaultPlacingRot, Faction.OfPlayer, null);
					}
				};
				if (!acceptanceReport.Accepted)
				{
					action.Disable(acceptanceReport.Reason);
				}
			}

			return action;
		}

		private static void LogOnce(string str)
		{
			Log.ErrorOnce(str, str.GetHashCode());
		}

		internal static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> gizmos, Building_PodLauncher __instance)
		{
			var extension = __instance.Map.Biome.GetModExtension<BiomesMap>();
			var isCavernMap = extension != null && extension.isCavern;
			var labelToRemove = isCavernMap ? "BuildThing".Translate(ThingDefOf.TransportPod.label).ToString() : null;
			foreach (var gizmo in gizmos)
			{
				// Remove the transport pod gizmo if the map is a cavern.
				if (!isCavernMap || !(gizmo is Command command) || command.Label != labelToRemove)
				{
					yield return gizmo;
				}
			}

			if (isCavernMap)
			{
				var drillPodGizmo = DrillPodGizmo(__instance);
				if (drillPodGizmo != null)
				{
					yield return drillPodGizmo;
				}
			}
		}
	}
}