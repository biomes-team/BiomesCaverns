using RimWorld;
using Verse;

namespace Caveworld_Flora_Unleashed
{
	public class Building_FungiponicsBasin : Building_PlantGrower
	{
		public override string GetInspectString()
		{
			float temperatureForCell = GenTemperature.GetTemperatureForCell(base.Position, base.Map);
			ThingDef_FruitingBody thingDef_FruitingBody = GetPlantDefToGrow() as ThingDef_FruitingBody;
			if (temperatureForCell < (float)thingDef_FruitingBody.minGrowTemperature)
			{
				return "Caveworld_Flora_Unleashed.CannotGrowTooCold".Translate();
			}
			if (temperatureForCell > (float)thingDef_FruitingBody.maxGrowTemperature)
			{
				return "Caveworld_Flora_Unleashed.CannotGrowTooHot".Translate();
			}
			return "Caveworld_Flora_Unleashed.Growing".Translate();
		}
	}
}
