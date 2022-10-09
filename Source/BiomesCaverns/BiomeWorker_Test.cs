using RimWorld;
using RimWorld.Planet;

namespace BiomesCaverns
{
    public class BiomeWorker_Test : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            if (tile.WaterCovered)
            {
                return -100;
            }
            return 100;

        }
    }











}
