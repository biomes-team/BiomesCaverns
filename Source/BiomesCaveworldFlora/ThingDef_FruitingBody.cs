using Verse;

namespace Caveworld_Flora_Unleashed
{
    public class ThingDef_FruitingBody : ThingDef
    {
        public bool growsOnlyInCaveBiome = false;

        public int minGrowTemperature = 0;

        public int minOptimalGrowTemperature = 10;

        public int maxOptimalGrowTemperature = 40;

        public int maxGrowTemperature = 50;

        public bool growOnlyOnRoughRock = false;

        public bool growOnlyUndeRoof = false;

        public bool growOnlyNearNaturalRock = false;

        public float minFertility = 0f;

        public float maxFertility = 999f;

        public float minLight = 0f;

        public float maxLight = 1f;

        public float MyceliumAbundance = 1f;

        public IntRange MyceliumSizeRange;

        public float MyceliumSpawnRadius = 1f;

        public float MyceliumExclusivityRadiusOffset = 1f;

        public float MyceliumExclusivityRadiusFactor = 0f;

    }
}