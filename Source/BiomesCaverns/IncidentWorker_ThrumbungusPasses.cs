using BiomesCore.DefModExtensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCaverns
{
	public class IncidentWorker_ThrumbungusPasses : IncidentWorker
	{
		public override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map) parms.target;
			var extension = map.Biome.GetModExtension<BiomesMap>();
			return extension != null && extension.isCavern &&
			       map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(BC_DefOf.BMT_Thrumbungus) &&
			       TryFindEntryCell(map, out _);
		}

		public override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map) parms.target;
			if (!TryFindEntryCell(map, out var cell))
			{
				return false;
			}

			PawnKindDef thrumbungus = BC_PawnDefOf.BMT_Thrumbungus;
			int value = GenMath.RoundRandom(StorytellerUtility.DefaultThreatPointsNow(map) / thrumbungus.combatPower);
			int max = Rand.RangeInclusive(3, 6);
			value = Mathf.Clamp(value, 2, max);
			int num = Rand.RangeInclusive(90000, 150000);

			if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(cell, map, 10f, out IntVec3 result))
			{
				result = IntVec3.Invalid;
			}

			Pawn pawn = null;
			for (int i = 0; i < value; i++)
			{
				IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, map, 10);
				pawn = PawnGenerator.GeneratePawn(thrumbungus);
				GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
				pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num;
				if (result.IsValid)
				{
					pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(result, map, 10);
				}
			}

			SendStandardLetter("BMT_LetterLabelThrumbungusPasses".Translate(thrumbungus.label).CapitalizeFirst(),
				"BMT_LetterThrumbungusPasses".Translate(thrumbungus.label), LetterDefOf.PositiveEvent, parms, pawn);
			return true;
		}

		private bool TryFindEntryCell(Map map, out IntVec3 cell)
		{
			return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
		}
	}
}