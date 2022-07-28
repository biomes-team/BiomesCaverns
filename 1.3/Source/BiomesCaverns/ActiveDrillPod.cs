using RimWorld;
using Verse;

namespace BiomesCaverns
{
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
}