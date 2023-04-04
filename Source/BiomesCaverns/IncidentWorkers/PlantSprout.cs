using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCaverns.IncidentWorkers
{
	public abstract class PlantSprout : IncidentWorker
	{
		protected abstract ThingDef PlantDef();

		protected virtual IntRange CountRange()
		{
			return new IntRange(10, 20);
		}

		protected virtual int MinRoomCells()
		{
			return 64;
		}

		protected virtual int SpawnRadius()
		{
			return 6;
		}

		protected virtual bool IgnoreSeason()
		{
			return false;
		}


		protected virtual bool AllowIndoors()
		{
			return false;
		}

		private bool CanSpawnAt(IntVec3 c, Map map)
		{
			ThingDef plantDef = PlantDef();
			if (!c.Standable(map) || c.Fogged(map) ||
			    map.fertilityGrid.FertilityAt(c) < plantDef.plant.fertilityMin ||
			    (!AllowIndoors() && !c.GetRoom(map).PsychologicallyOutdoors) || c.GetEdifice(map) != null ||
			    (!IgnoreSeason() && !PlantUtility.GrowthSeasonNow(c, map)))
			{
				return false;
			}

			Plant plant = c.GetPlant(map);
			if (plant != null && plant.def.plant.growDays > 10.0F)
			{
				return false;
			}

			List<Thing> thingList = c.GetThingList(map);
			foreach (Thing cellThing in thingList)
			{
				if (cellThing.def == plantDef)
				{
					return false;
				}
			}

			return true;
		}

		private bool TryFindRootCell(Map map, out IntVec3 cell)
		{
			return CellFinderLoose.TryFindRandomNotEdgeCellWith(10,
				x => CanSpawnAt(x, map) && x.GetRoom(map).CellCount >= MinRoomCells(), map, out cell);
		}


		public override bool CanFireNowSub(IncidentParms parms)
		{
			Map target = parms.target as Map;
			/*
			Log.Error(
			$"CanFire|target {target}|base {base.CanFireNowSub(parms)}|IgnoreSeason:{IgnoreSeason()}|GrowthSeasonOutdoorsNow:{target != null && target.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow}|find:{TryFindRootCell(target, out IntVec3 _)}");
			*/
			return target != null && base.CanFireNowSub(parms) &&
			       (IgnoreSeason() || target.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow) &&
			       TryFindRootCell(target, out IntVec3 _);
		}

		public override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map) parms.target;
			if (!TryFindRootCell(map, out IntVec3 rootCell))
			{
				return false;
			}

			Thing anyPlant = null;
			int randomInRange = CountRange().RandomInRange;
			for (int index = 0; index < randomInRange; ++index)
			{
				CellFinder.TryRandomClosewalkCellNear(rootCell, map, SpawnRadius(), out IntVec3 result,
					x => CanSpawnAt(x, map));
				result.GetPlant(map)?.Destroy();
				anyPlant = GenSpawn.Spawn(PlantDef(), result, map);
			}

			bool success = anyPlant != null;
			if (success)
			{
				SendStandardLetter(parms, anyPlant);
			}

			return true;
		}
	}
}