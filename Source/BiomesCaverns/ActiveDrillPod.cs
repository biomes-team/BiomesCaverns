using RimWorld;

namespace BiomesCaverns
{
	public class ActiveDrillPod : ActiveTransporter
	{
		protected override void Tick()
		{
			if (Contents != null)
			{
                Contents.innerContainer.DoTick();
            }

			base.Tick();
		}
	}
}