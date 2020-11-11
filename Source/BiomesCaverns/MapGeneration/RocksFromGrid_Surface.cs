using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BiomesCaverns.MapGeneration
{
    public class RocksFromGrid_Surface : GenStep
    {

        private class RoofThreshold
        {
            public RoofDef roofDef;

            public float minGridVal;
        }

        private float maxMineableValue = 3.40282347E+38f;

        private const int MinRoofedCellsPerGroup = 20;

        public override int SeedPart
        {
            get
            {
                return 1182952823;
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
            Log.Message("Caves");

            map.regionAndRoomUpdater.Enabled = false;
            float num = 0.7f;
            List<RoofThreshold> list = new List<RoofThreshold>();
            list.Add(new RoofThreshold
            {
                roofDef = BiomesCavernsDefOf.BiomesCaverns_RockRoofStable,
                minGridVal = 0.33f
            });
            //list.Add(new RoofThreshold
            //{
            //    roofDef = RoofDefOf.RoofRockThick,
            //    //minGridVal = num * 1.14f
            //    minGridVal = 0.33f
            //});
            list.Add(new RoofThreshold
            {
                roofDef = RoofDefOf.RoofRockThin,
                //minGridVal = num * 1.04f
                minGridVal = 0.20f
            });

            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid caves = MapGenerator.Caves;
            foreach (IntVec3 current in map.AllCells)
            {
                float num2 = elevation[current];
                if (num2 > num)
                {
                    if (caves[current] <= 0f)
                    {
                        ThingDef def = GenStep_RocksFromGrid.RockDefAt(current);
                        GenSpawn.Spawn(def, current, map, WipeMode.Vanish);
                    }
                }
                //moved to here so that roofs are applied to tiles with no rocks
                for (int i = 0; i < list.Count; i++)
                {
                    if (num2 > list[i].minGridVal)
                    {
                        map.roofGrid.SetRoof(current, list[i].roofDef);
                        break;
                    }
                }
            }
            BoolGrid visited = new BoolGrid(map);
            List<IntVec3> toRemove = new List<IntVec3>();
            foreach (IntVec3 current2 in map.AllCells)
            {
                if (!visited[current2])
                {
                    if (this.IsNaturalRoofAt(current2, map))
                    {
                        toRemove.Clear();
                        map.floodFiller.FloodFill(current2, (IntVec3 x) => this.IsNaturalRoofAt(x, map), delegate (IntVec3 x)
                        {
                            visited[x] = true;
                            toRemove.Add(x);
                        }, 2147483647, false, null);
                        if (toRemove.Count < 20)
                        {
                            for (int j = 0; j < toRemove.Count; j++)
                            {
                                map.roofGrid.SetRoof(toRemove[j], null);
                            }
                        }
                    }
                }
            }
            GenStep_ScatterLumpsMineable genStep_ScatterLumpsMineable = new GenStep_ScatterLumpsMineable();
            genStep_ScatterLumpsMineable.maxValue = this.maxMineableValue;
            float num3 = 10f;
            switch (Find.WorldGrid[map.Tile].hilliness)
            {
                case Hilliness.Flat:
                    num3 = 4f;
                    break;
                case Hilliness.SmallHills:
                    num3 = 8f;
                    break;
                case Hilliness.LargeHills:
                    num3 = 11f;
                    break;
                case Hilliness.Mountainous:
                    num3 = 15f;
                    break;
                case Hilliness.Impassable:
                    num3 = 16f;
                    break;
            }
            genStep_ScatterLumpsMineable.countPer10kCellsRange = new FloatRange(num3, num3);
            genStep_ScatterLumpsMineable.Generate(map, parms);
            map.regionAndRoomUpdater.Enabled = true;
        }

        private bool IsNaturalRoofAt(IntVec3 c, Map map)
        {
            return c.Roofed(map) && c.GetRoof(map).isNatural;
        }

    }
}
