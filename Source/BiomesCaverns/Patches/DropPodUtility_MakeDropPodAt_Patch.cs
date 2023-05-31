using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using BiomesCore;
using Verse;

namespace BiomesCaverns
{
    [HarmonyPatch(typeof(DropPodUtility), "MakeDropPodAt")]
    public static class DropPodUtility_MakeDropPodAt_Patch
    {
        public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            // Certain mods might trigger drop pods outside of the valid bounds of the map.
            if (!c.InBounds(map))
            {
                return false;
            }

            if (map.roofGrid.RoofAt(c) == BiomesCoreDefOf.BMT_RockRoofStable)
            {
                MakeDrillPod(c, map, info);
                return false;
            }
            
            if (info.innerContainer.Any(x => x.def == BC_DefOf.BMT_DrillPod))
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