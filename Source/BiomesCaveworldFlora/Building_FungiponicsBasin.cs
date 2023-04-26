using RimWorld;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class Building_FungiponicsBasin : Building_PlantGrower
    {
        public override string GetInspectString()
        {
            float temperatureForCell = GenTemperature.GetTemperatureForCell(Position, Map);
            var thingDef_FruitingBody = GetPlantDefToGrow() as ThingDef_FruitingBody;
            if (temperatureForCell < thingDef_FruitingBody.minGrowTemperature)
            {
                return "Caveworld_Flora_Unleashed.CannotGrowTooCold".Translate();
            }

            if (temperatureForCell > thingDef_FruitingBody.maxGrowTemperature)
            {
                return "Caveworld_Flora_Unleashed.CannotGrowTooHot".Translate();
            }

            return "Caveworld_Flora_Unleashed.Growing".Translate();
        }
    }
}
