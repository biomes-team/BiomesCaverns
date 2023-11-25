using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCaverns.Buildings
{
	public class Building_FungalPowerGenerator : Building
	{
		public IEnumerable<IntVec3> GrowableCells => GenRadial.RadialCellsAround(Position, def.specialDisplayRadius, true);
		
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Growing>() != null)
			{
				Command_Action gizmo = new Command_Action();
				gizmo.action = MakeMatchingGrowZone;
				gizmo.hotKey = KeyBindingDefOf.Misc2;
				// ToDo
				gizmo.defaultDesc = "BMT_FungalPowerGenerator_MakeGrowingZoneDesc".Translate();
				gizmo.icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing");
				// ToDo
				gizmo.defaultLabel = "CommandSunLampMakeGrowingZoneLabel".Translate();
				yield return gizmo;
			}
		}

		private void MakeMatchingGrowZone()
		{
			Designator designator = DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Growing>();
			designator.DesignateMultiCell(GrowableCells.Where(tempCell => designator.CanDesignateCell(tempCell).Accepted));
		}

	}
}