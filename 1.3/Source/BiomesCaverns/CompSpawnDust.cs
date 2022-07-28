using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCaverns
{
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
}