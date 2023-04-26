using RimWorld;
using UnityEngine;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class FruitingBody_Gleamcap : FruitingBody
    {
        public const float poisonRadius = 2f;

        public const float minGrowthToPoison = 0.6f;

        public int nextLongTick = 2000;

        public static bool alertHasBeenSent;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextLongTick, "nextLongTick");
        }

        public override void TickRare()
        {
            if (Growth >= 0.6f && !Dying && !IsInCryostasis)
            {
                ThrowPoisonSmoke();
                var allPawnsSpawned = Map.mapPawns.AllPawnsSpawned;
                foreach (var pawn in allPawnsSpawned)
                {
                    if (!pawn.Position.InHorDistOf(Position, 2f) || !pawn.RaceProps.IsFlesh)
                    {
                        continue;
                    }

                    {
                        float num = 0.00287583f;
                        num *= Mathf.Clamp01(1 - pawn.GetStatValue(StatDefOf.ToxicResistance));
                        if (num != 0f)
                        {
                            float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 0x46EDC5D));
                            num *= num2;
                            HealthUtility.AdjustSeverity(pawn, Util_Caveworld_Flora_Unleashed.gleamcapSmokeDef, num);
                        }
                    }
                }
            }

            if (Find.TickManager.TicksGame >= nextLongTick)
            {
                nextLongTick = Find.TickManager.TicksGame + 2500;
                TickLong();
            }
        }

        public void ThrowPoisonSmoke()
        {
            var spawnPosition = Position.ToVector3Shifted() + Vector3Utility.RandomHorizontalOffset(3f);
            if (spawnPosition.ShouldSpawnMotesAt(Map) && !Map.moteCounter.SaturatedLowPriority)
            {
                var moteThrown = ThingMaker.MakeThing(Util_Caveworld_Flora_Unleashed.MoteGleamcapSmokeDef) as MoteThrown;
                moteThrown.Scale = 3f * Growth;
                moteThrown.rotationRate = Rand.Range(-4, 4);
                moteThrown.exactPosition = spawnPosition;
                moteThrown.SetVelocity(Rand.Range(-20, 20), 0f);
                GenSpawn.Spawn(moteThrown, spawnPosition.ToIntVec3(), Map);
            }
        }
    }
}
