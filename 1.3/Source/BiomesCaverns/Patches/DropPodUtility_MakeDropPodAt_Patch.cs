using BiomesCore.DefModExtensions;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCaverns
{
    [DefOf]
    public static class BC_DefOf
    {
        public static ThingDef BMT_DrillPodLeaving;
        public static ThingDef BMT_DrillPodIncoming;
        public static ThingDef BMT_DrillPodActive;
    }

    public class CompSpawnDust : ThingComp
    {
        private static float DustMoteSpawnMTB = 0.2f;
        private static float FilthSpawnMTB = 0.3f;
        private static float FilthSpawnRadius = 3f;

        private static List<ThingDef> filthTypes = new List<ThingDef>
        {
            ThingDefOf.Filth_Dirt,ThingDefOf.Filth_Dirt, ThingDefOf.Filth_Dirt, ThingDefOf.Filth_RubbleRock
        };
        public override void CompTick()
        {
            base.CompTick();
            Vector3 vector = base.parent.PositionHeld.ToVector3Shifted();
            if (Rand.MTBEventOccurs(FilthSpawnMTB, 1f, 1.TicksToSeconds())
                && CellFinder.TryFindRandomReachableCellNear(base.parent.PositionHeld, base.parent.Map, FilthSpawnRadius,
                TraverseParms.For(TraverseMode.NoPassClosedDoors), null, null, out var result))
            {
                FilthMaker.TryMakeFilth(result, base.parent.Map, filthTypes.RandomElement());
            }
            if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
            {
                Vector3 loc = new Vector3(vector.x, 0f, vector.z);
                loc.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                FleckMaker.ThrowDustPuffThick(loc, base.parent.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
            }

        }
    }
    [StaticConstructorOnStartup]
    public class ActiveDrillPod : ActiveDropPod
    {
        public override void Tick()
        {
            if (contents == null)
            {
                return;
            }
            contents.innerContainer.ThingOwnerTick();
            if (base.Spawned)
            {
                age++;
                if (age > contents.openDelay)
                {
                    PodOpen();
                }
            }
        }
    }
    [HarmonyPatch(typeof(DropPodUtility), "MakeDropPodAt")]
    public static class DropPodUtility_MakeDropPodAt_Patch
    {
        public static bool Prefix(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            if (map.Biome.HasModExtension<BiomesMap>())
            {
                BiomesMap biome = map.Biome.GetModExtension<BiomesMap>();
                if (biome.isCavern)
                {
                    ActiveDropPod activeDropPod = (ActiveDrillPod)ThingMaker.MakeThing(BC_DefOf.BMT_DrillPodActive);
                    activeDropPod.Contents = info;
                    SkyfallerMaker.SpawnSkyfaller(BC_DefOf.BMT_DrillPodIncoming, activeDropPod, c, map);
                    foreach (Thing item in (IEnumerable<Thing>)activeDropPod.Contents.innerContainer)
                    {
                        Pawn pawn;
                        if ((pawn = item as Pawn) != null && pawn.IsWorldPawn())
                        {
                            Find.WorldPawns.RemovePawn(pawn);
                            pawn.psychicEntropy?.SetInitialPsyfocusLevel();
                        }
                    }
                    return false;
                }
            }
            return true;
        }
    }
}