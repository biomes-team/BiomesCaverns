using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCaverns
{
    [HarmonyPatch(typeof(CompLaunchable), "TryLaunch")]
    public class CompLaunchable_TryLaunch_Patch
    {
        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(CompLaunchable __instance, int destinationTile, TransportPodsArrivalAction arrivalAction)
        {
            if (__instance.parent.def == BC_DefOf.BMT_DrillPod)
            {
                TryLaunch(__instance, destinationTile, arrivalAction);
                return false;
            }
            return true;
        }

        public static void TryLaunch(CompLaunchable __instance, int destinationTile, TransportPodsArrivalAction arrivalAction)
        {
            if (!__instance.parent.Spawned)
            {
                Log.Error(string.Concat("Tried to launch ", __instance.parent, ", but it's unspawned."));
                return;
            }
            List<CompTransporter> transportersInGroup = __instance.TransportersInGroup;
            if (transportersInGroup == null)
            {
                Log.Error(string.Concat("Tried to launch ", __instance.parent, ", but it's not in any group."));
            }
            else
            {
                if (!__instance.LoadingInProgressOrReadyToLaunch || !__instance.AllInGroupConnectedToFuelingPort || !__instance.AllFuelingPortSourcesInGroupHaveAnyFuel)
                {
                    return;
                }
                Map map = __instance.parent.Map;
                int num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, destinationTile);
                if (num <= __instance.MaxLaunchDistance)
                {
                    __instance.Transporter.TryRemoveLord(map);
                    int groupID = __instance.Transporter.groupID;
                    float amount = Mathf.Max(CompLaunchable.FuelNeededToLaunchAtDist(num), 1f);
                    for (int i = 0; i < transportersInGroup.Count; i++)
                    {
                        CompTransporter compTransporter = transportersInGroup[i];
                        compTransporter.Launchable.FuelingPortSource?.TryGetComp<CompRefuelable>().ConsumeFuel(amount);
                        ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();
                        ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(BC_DefOf.BMT_DrillPodActive);
                        activeDropPod.Contents = new ActiveDropPodInfo();
                        activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, canMergeWithExistingStacks: true, destroyLeftover: true);
                        compTransporter.parent.DeSpawn();
                        activeDropPod.Contents.innerContainer.TryAddOrTransfer(compTransporter.parent);
                        FlyShipLeaving obj = (FlyShipLeaving)SkyfallerMaker.MakeSkyfaller(__instance.Props.skyfallerLeaving, activeDropPod);
                        obj.groupID = groupID;
                        obj.destinationTile = destinationTile;
                        obj.arrivalAction = arrivalAction;
                        obj.worldObjectDef = BC_DefOf.BMT_TravelingDrillPods;
                        compTransporter.CleanUpLoadingVars(map);
                        GenSpawn.Spawn(obj, compTransporter.parent.Position, map);
                    }
                    CameraJumper.TryHideWorld();
                }
            }
        }
    }
}