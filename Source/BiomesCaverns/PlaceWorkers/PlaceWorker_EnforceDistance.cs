using System.Collections.Generic;
using Verse;

namespace BiomesCaverns.PlaceWorkers
{
	public class PlaceWorker_EnforceDistance : PlaceWorker
	{
		private const int Distance = 5;

		public override AcceptanceReport AllowsPlacing(
			BuildableDef def,
			IntVec3 center,
			Rot4 rot,
			Map map,
			Thing thingToIgnore = null,
			Thing thing = null)
		{
			foreach (IntVec3 cell in GenRadial.RadialCellsAround(center, Distance, true))
				// foreach (IntVec3 cell in GenAdj.OccupiedRect(center, rot, def.Size).ExpandedBy(Distance))
			{
				if (!cell.InBounds(map))
				{
					continue;
				}

				List<Thing> thingList = cell.GetThingList(map);
				for (int index = 0; index < thingList.Count; ++index)
				{
					if (thingList[index].def == def || thingList[index].def.entityDefToBuild == def)
					{
						return "BMT_FungalPowerGenerator_MustNotBePlacedCloseToAnother".Translate(Distance,
							(NamedArgument) (Def) def);
					}
				}
			}

			return true;
		}
	}
}