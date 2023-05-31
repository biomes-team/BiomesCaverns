using BiomesCore;
using RimWorld;
using Verse;

namespace BiomesCaverns
{
	public class ShuttleIncomingCavernRoof : ShuttleIncoming
	{
		protected override void HitRoof()
		{
			if (Position.GetRoof(Map) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				return;
			}

			base.HitRoof();
		}
	}
}