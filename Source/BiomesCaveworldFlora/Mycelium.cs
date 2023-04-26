using System.Text;
using RimWorld;
using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class Mycelium : Plant
    {
        public ThingDef_FruitingBody plantDef;

        public int actualSize;

        public int desiredSize;

        public int nextGrownTick;

        public int nextReproductionTick;

        public float ExclusivityRadius => plantDef.MyceliumExclusivityRadiusOffset + desiredSize * plantDef.MyceliumExclusivityRadiusFactor;

        public override string LabelMouseover => def.LabelCap;

        public static FruitingBody SpawnNewMyceliumAt(Map map, IntVec3 spawnCell, ThingDef_FruitingBody plantDef, int desiredSize)
        {
            var newPlant = ThingMaker.MakeThing(plantDef) as FruitingBody;
            GenSpawn.Spawn(newPlant, spawnCell, map);
            var newMycelium = ThingMaker.MakeThing(Util_Caveworld_Flora_Unleashed.MyceliumDef) as Mycelium;
            newMycelium.Initialize(plantDef, desiredSize);
            GenSpawn.Spawn(newMycelium, spawnCell, map);
            newPlant.Mycelium = newMycelium;
            return newPlant;
        }

        public void Initialize(ThingDef_FruitingBody plantDef, int desiredSize)
        {
            Growth = 1f;
            this.plantDef = plantDef;
            actualSize = 1;
            this.desiredSize = desiredSize;
        }

        public static float GetExclusivityRadius(ThingDef_FruitingBody plantDef, int MyceliumSize)
        {
            return plantDef.MyceliumExclusivityRadiusOffset + MyceliumSize * plantDef.MyceliumExclusivityRadiusFactor;
        }

        public static float GetMaxExclusivityRadius(ThingDef_FruitingBody plantDef)
        {
            return plantDef.MyceliumExclusivityRadiusOffset + plantDef.MyceliumSizeRange.max * plantDef.MyceliumExclusivityRadiusFactor;
        }

        public override void TickLong()
        {
            if (Find.TickManager.TicksGame > nextGrownTick && FruitingBody.IsTemperatureConditionOkAt(plantDef, Map, Position) && FruitingBody.IsLightConditionOkAt(plantDef, Map, Position))
            {
                nextGrownTick = Find.TickManager.TicksGame + (int) (plantDef.plant.lifespanDaysPerGrowDays * 60000f);
                _ = GenCaveFungusReproduction.TryGrowMycelium(this);
            }

            if (actualSize == desiredSize && Find.TickManager.TicksGame > nextReproductionTick && FruitingBody.IsTemperatureConditionOkAt(plantDef, Map, Position) && FruitingBody.IsLightConditionOkAt(plantDef, Map, Position))
            {
                GenCaveFungusReproduction.TrySpawnNewMyceliumAwayFrom(this);
                nextReproductionTick = Find.TickManager.TicksGame + (int) (plantDef.plant.lifespanDaysPerGrowDays * 10f * 60000f);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            string plantDefAsString = "";
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                plantDefAsString = plantDef.defName;
                Scribe_Values.Look(ref plantDefAsString, "plantDefAsString");
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Scribe_Values.Look(ref plantDefAsString, "plantDefAsString");
                plantDef = ThingDef.Named(plantDefAsString) as ThingDef_FruitingBody;
            }

            Scribe_Values.Look(ref actualSize, "actualSize");
            Scribe_Values.Look(ref desiredSize, "desiredSize");
            Scribe_Values.Look(ref nextReproductionTick, "nextGrownTick");
            Scribe_Values.Look(ref nextReproductionTick, "nextReproductionTick");
        }

        public void NotifyPlantAdded()
        {
            actualSize++;
        }

        public void NotifyPlantRemoved()
        {
            actualSize--;
            if (actualSize <= 0)
            {
                Destroy();
            }
        }

        public override string GetInspectString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(plantDef.LabelCap);
            return stringBuilder.ToString();
        }
    }
}
