using RimWorld;

namespace BiomesCaverns
{
	public class ActiveDrillPod : ActiveDropPod
	{
		public override void Tick()
		{
			if (contents == null)
			{
				return;
			}

			contents.innerContainer.ThingOwnerTick();
			if (Spawned)
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