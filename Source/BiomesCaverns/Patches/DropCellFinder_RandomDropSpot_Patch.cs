using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Allow drillpods to choose landing spots under cavern roof.
	/// </summary>
	[HarmonyPatch(typeof(DropCellFinder), nameof(DropCellFinder.RandomDropSpot))]
	public static class DropCellFinder_RandomDropSpot_Patch
	{
		public static bool Prefix(ref IntVec3 __result, Map map, bool standableOnly = true)
		{
			if (TravelingTransportPods_Arrived_Patch.drillPodIncoming || map?.Biome?.GetModExtension<BiomesCore.DefModExtensions.BiomesMap>()?.isCavern == true)
			{
				//Log.Error("0");
				__result = CellFinderLoose.RandomCellWith((IntVec3 c) => (!standableOnly || c.Standable(map)) && !c.Fogged(map),
					map);
				return false;
			}

			return true;
		}
	}

    //[HarmonyPatch(typeof(CellFinderLoose), nameof(CellFinderLoose.TryFindSkyfallerCell))]
    //public static class CellFinderLoose_TryFindSkyfallerCell_Patch
    //{
    //	public static bool Prefix(ref bool __result, ThingDef skyfaller, Map map, int minDistToEdge, ref IntVec3 cell)
    //	{
    //		if (map?.Biome?.GetModExtension<BiomesCore.DefModExtensions.BiomesMap>()?.isCavern == true)
    //		{
    //               bool validator(IntVec3 x)
    //               {
    //                   foreach (IntVec3 item in GenAdj.OccupiedRect(x, Rot4.North, skyfaller.size))
    //                   {
    //                       if (!item.InBounds(map) || item.Fogged(map) || !item.Standable(map))
    //                       {
    //                           return false;
    //					}
    //					foreach (Thing thing in item.GetThingList(map))
    //					{
    //						if (thing.def.preventSkyfallersLandingOn)
    //						{
    //							return false;
    //						}
    //					}
    //				}
    //				if (!map.reachability.CanReachColony(x))
    //				{
    //					return false;
    //				}
    //				return true;
    //			}
    //			__result = CellFinderLoose.TryFindRandomNotEdgeCellWith(minDistToEdge, validator, map, out cell);
    //			return !__result;
    //		}
    //		return true;
    //	}

    //}

    [HarmonyPatch(typeof(LargeBuildingCellFinder), nameof(LargeBuildingCellFinder.TryFindCell))]
    public static class LargeBuildingCellFinder_TryFindCell_Patch
    {
        public static bool Prefix(Map map, ref LargeBuildingSpawnParms parms)
        {
            if (map?.Biome?.GetModExtension<BiomesCore.DefModExtensions.BiomesMap>()?.isCavern == true)
            {
                parms.attemptSpawnLocationType = SpawnLocationType.Anywhere;
            }
            return true;
        }

    }

}