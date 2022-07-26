using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace BiomesCaverns
{
    [StaticConstructorOnStartup]
    public static class BiomesCaverns
    {
        public const string Id = "rimworld.biomes.caverns";
        
        static BiomesCaverns()
        {
            new Harmony(Id).PatchAll();
            Log.Message("Biomes! Caverns initialized");
        }
    }
}