using BiomesCore.DefModExtensions;
using BiomesCore.Incidents;
using RimWorld;
using Verse;

namespace BiomesCaverns.Incidents
{
	public class IncidentWorker_HungeringHydra : IncidentWorker_HungeringAnimals
	{
		protected override PawnKindDef HungeringAnimalDef(IncidentParms parms)
		{
			if (parms.target is Map map)
			{
				var extension = map.Biome.GetModExtension<BiomesMap>();
				if (extension != null && extension.isCavern)
				{
					return BC_PawnDefOf.BMT_HungeringHydra;
				}
			}

			return null;
		}
	}
}