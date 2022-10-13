using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BiomesCaverns
{
    public class DrillPodIncoming : Skyfaller, IActiveDropPod
    {
        public ActiveDropPodInfo Contents
        {
            get => ((ActiveDropPod)innerContainer.GetAt(0)).Contents;
            set => ((ActiveDropPod)innerContainer.GetAt(0)).Contents = value;
        }

        public override void SpawnThings()
        {
            if (!Contents.spawnWipeMode.HasValue)
            {
                base.SpawnThings();
                return;
            }
            for (int num = innerContainer.Count - 1; num >= 0; num--)
            {
                GenSpawn.Spawn(innerContainer.GetAt(num), Position, Map, Contents.spawnWipeMode.Value);
            }
        }

        public override void HitRoof()
        {

        }

        public override void Impact()
        {
            if (def.skyfaller.CausesExplosion)
            {
                GenExplosion.DoExplosion(Position, Map, def.skyfaller.explosionRadius, def.skyfaller.explosionDamage, null, GenMath.RoundRandom(def.skyfaller.explosionDamage.defaultDamage * def.skyfaller.explosionDamageFactor), -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false, null, (!def.skyfaller.damageSpawnedThings) ? innerContainer.ToList() : null);
            }
            SpawnThings();
            innerContainer.ClearAndDestroyContents();
            CellRect cellRect = this.OccupiedRect();
            for (int i = 0; i < cellRect.Area * def.skyfaller.motesPerCell; i++)
            {
                FleckMaker.ThrowDustPuff(cellRect.RandomVector3, Map, 2f);
            }
            if (def.skyfaller.MakesShrapnel)
            {
                SkyfallerShrapnelUtility.MakeShrapnel(Position, Map, shrapnelDirection, def.skyfaller.shrapnelDistanceFactor, def.skyfaller.metalShrapnelCountRange.RandomInRange, def.skyfaller.rubbleShrapnelCountRange.RandomInRange, spawnMotes: true);
            }
            if (def.skyfaller.cameraShake > 0f && Map == Find.CurrentMap)
            {
                Find.CameraDriver.shaker.DoShake(def.skyfaller.cameraShake);
            }

            def.skyfaller.impactSound?.PlayOneShot(SoundInfo.InMap(new TargetInfo(Position, Map)));
            Destroy();
        }
    }
}