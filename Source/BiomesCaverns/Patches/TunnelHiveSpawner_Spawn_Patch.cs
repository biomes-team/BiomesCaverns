using HarmonyLib;
using RimWorld;
using Verse;

namespace BiomesCaverns.Patches
{
	[HarmonyPatch(typeof(TunnelHiveSpawner), "Spawn")]
	internal static class TunnelHiveSpawner_Spawn_Patch
	{
		public static void Postfix(Map map, IntVec3 loc)
		{
			if (Rand.Chance(0.75F)) // 25% chance of spawning a pod worm.
			{
				return;
			}

			var request = new PawnGenerationRequest(BC_PawnDefOf.BMT_PodWorm);
			var lifeStageIndex = BC_PawnDefOf.BMT_PodWorm.lifeStages.Count - 1;
			request.FixedBiologicalAge = BC_PawnDefOf.BMT_PodWorm.race.race.lifeStageAges[lifeStageIndex].minAge;
			var pawn = PawnGenerator.GeneratePawn(request);
			if (pawn != null)
			{
				GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(loc, map, 2), map);
			}
		}
	}
}