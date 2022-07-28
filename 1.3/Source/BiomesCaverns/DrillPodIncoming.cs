using RimWorld;
using System.Linq;
using Verse;
using Verse.Sound;

namespace BiomesCaverns
{
    public class DrillPodIncoming : Skyfaller, IActiveDropPod, IThingHolder
    {
        public ActiveDropPodInfo Contents
        {
            get
            {
                return ((ActiveDropPod)innerContainer[0]).Contents;
            }
            set
            {
                ((ActiveDropPod)innerContainer[0]).Contents = value;
            }
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
                GenSpawn.Spawn(innerContainer[num], base.Position, base.Map, Contents.spawnWipeMode.Value);
            }
        }

        public override void HitRoof()
        {

        }

        public override void Impact()
        {
            if (def.skyfaller.CausesExplosion)
            {
                GenExplosion.DoExplosion(base.Position, base.Map, def.skyfaller.explosionRadius, def.skyfaller.explosionDamage, null, GenMath.RoundRandom((float)def.skyfaller.explosionDamage.defaultDamage * def.skyfaller.explosionDamageFactor), -1f, null, null, null, null, null, 0f, 1, applyDamageToExplosionCellsNeighbors: false, null, 0f, 1, 0f, damageFalloff: false, null, (!def.skyfaller.damageSpawnedThings) ? innerContainer.ToList() : null);
            }
            SpawnThings();
            innerContainer.ClearAndDestroyContents();
            CellRect cellRect = this.OccupiedRect();
            for (int i = 0; i < cellRect.Area * def.skyfaller.motesPerCell; i++)
            {
                FleckMaker.ThrowDustPuff(cellRect.RandomVector3, base.Map, 2f);
            }
            if (def.skyfaller.MakesShrapnel)
            {
                SkyfallerShrapnelUtility.MakeShrapnel(base.Position, base.Map, shrapnelDirection, def.skyfaller.shrapnelDistanceFactor, def.skyfaller.metalShrapnelCountRange.RandomInRange, def.skyfaller.rubbleShrapnelCountRange.RandomInRange, spawnMotes: true);
            }
            if (def.skyfaller.cameraShake > 0f && base.Map == Find.CurrentMap)
            {
                Find.CameraDriver.shaker.DoShake(def.skyfaller.cameraShake);
            }
            if (def.skyfaller.impactSound != null)
            {
                def.skyfaller.impactSound.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map)));
            }
            Destroy();
        }
    }
}