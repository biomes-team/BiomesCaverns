using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;

namespace BiomesCaverns
{
    public class BiomeWorker_NoTest : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {

            return -100;

        }
    }











}
