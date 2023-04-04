using Verse;

namespace BiomesCaverns.IncidentWorkers
{
	public class AmbrosyxSprout : PlantSprout
	{
		protected override ThingDef PlantDef()
		{
			return BC_DefOf.BMT_AmbrosyxFungus;
		}

		protected override bool IgnoreSeason()
		{
			return true;
		}
		
		protected override bool AllowIndoors()
		{
			return true;
		}
	}
}