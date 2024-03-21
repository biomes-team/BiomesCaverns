using System.Linq;
using BiomesCore;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(DropPodUtility), nameof(DropPodUtility.MakeDropPodAt))]
	public static class DropPodUtility_MakeDropPodAt_Patch
	{
		private static bool ShouldCreateDrillpod(IntVec3 cell, Map map, ActiveDropPodInfo info)
		{
			if (map.roofGrid.RoofAt(cell) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				return true;
			}

			foreach (Thing thing in info.innerContainer)
			{
				if (thing.def == BC_DefOf.BMT_DrillPod)
				{
					return true;
				}
			}

			return false;
		}

		public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
		{
			// Certain mods might trigger drop pods outside of the valid bounds of the map.
			if (!c.InBounds(map))
			{
				return false;
			}

			if (ShouldCreateDrillpod(c, map, info))
			{
				MakeDrillPod(c, map, info);
				return false;
			}

			return true;
		}

		private static void MakeDrillPod(IntVec3 c, Map map, ActiveDropPodInfo info)
		{
			ActiveDropPod activeDropPod = (ActiveDrillPod) ThingMaker.MakeThing(BC_DefOf.BMT_DrillPodActive);
			activeDropPod.Contents = info;

			SkyfallerMaker.SpawnSkyfaller(BC_DefOf.BMT_DrillPodIncoming, activeDropPod, c, map);

			foreach (var item in activeDropPod.Contents.innerContainer)
			{
				Pawn pawn;
				if ((pawn = item as Pawn) != null && pawn.IsWorldPawn())
				{
					Find.WorldPawns.RemovePawn(pawn);
					pawn.psychicEntropy?.SetInitialPsyfocusLevel();
				}
			}
		}
	}
}