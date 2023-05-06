using BiomesCore;
using Verse;

namespace BiomesCaverns.PlaceWorkers
{
	public class NotUnderRoof : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(
			BuildableDef checkingDef,
			IntVec3 loc,
			Rot4 rot,
			Map map,
			Thing thingToIgnore = null,
			Thing thing = null)
		{
			var roof = loc.GetRoof(map);
			return roof != null && roof != BiomesCoreDefOf.BMT_RockRoofStable
				? new AcceptanceReport("MustPlaceUnroofed".Translate())
				: (AcceptanceReport) true;
		}
	}
}