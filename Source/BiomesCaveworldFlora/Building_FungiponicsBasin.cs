using RimWorld;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class Building_FungiponicsBasin : Building_PlantGrower
    {
        public override string GetInspectString()
        {
            if (GetPlantDefToGrow() is ThingDef_FruitingBody thingDef)
            {
                float temperatureForCell = GenTemperature.GetTemperatureForCell(Position, Map);
            
                if (temperatureForCell < thingDef.minGrowTemperature)
                {
                    return "Caveworld_Flora_Unleashed.CannotGrowTooCold".Translate();
                }

                if (temperatureForCell > thingDef.maxGrowTemperature)
                {
                    return "Caveworld_Flora_Unleashed.CannotGrowTooHot".Translate();
                }
            }
            
            return "Caveworld_Flora_Unleashed.Growing".Translate();
        }
    }
}
