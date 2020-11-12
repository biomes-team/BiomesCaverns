using BiomesCaverns.MapGeneration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BiomesCaverns.Patches
{
    /// <summary>
    ///  Picks color for custom roofs, when roof overlay is toggled
    ///  Vanilla: thin roof = white, thick roof = 50% grey
    /// </summary>
    [HarmonyPatch(typeof(Verse.RoofGrid), "GetCellExtraColor")]
    static class RoofColorPatch
    {
        static void Postfix(int index, ref RoofGrid __instance, ref Color __result)
        {
            if (BiomesCavernsDefOf.BiomesCaverns_RockRoofStable != null && __instance.RoofAt(index) == BiomesCavernsDefOf.BiomesCaverns_RockRoofStable)
            {
                //__result = Color.black;

                // light grey
                //__result = new Color(0.75f, 0.75f, 0.75f, 1f);

                // dark grey
                __result = new Color(0.25f, 0.25f, 0.25f, 1f);
            }

        }
    }


    /// <summary>
    /// Allows placement of the cave roofs on map generation
    /// </summary>
    [HarmonyPatch(typeof(RimWorld.GenStep_RocksFromGrid), "Generate")]
    static class CaveRoofGeneration
    {
        static bool Prefix(Map map, GenStepParams parms)
        {
            if(map.Biome.defName == "BiomesCaverns_SurfaceCavern")
            {
                new RocksFromGrid_Surface().Generate(map, parms);
                return false;
            }

            return true;
        }

    }


    /// <summary>
    /// cave roofs don't have to be within range of a wall
    /// </summary>
    [HarmonyPatch(typeof(Verse.RoofCollapseUtility), "WithinRangeOfRoofHolder")]
    static class RoofCollapse_Disable
    {
        static bool Prefix(IntVec3 c, Map map, ref bool __result)
        {
            if(map.roofGrid.RoofAt(c) == BiomesCavernsDefOf.BiomesCaverns_RockRoofStable)
            {
                __result = true;
                return false;
            }
            return true;
        }

    }

    /// <summary>
    /// Lowers infestation chance under cave roofs
    /// </summary>
    [HarmonyPatch(typeof(RimWorld.InfestationCellFinder), "GetMountainousnessScoreAt")]
    static class InfestationModifier
    {
        static void Postfix(IntVec3 cell, Map map, ref float __result)
        {
            if (map.roofGrid.RoofAt(cell) == BiomesCavernsDefOf.BiomesCaverns_RockRoofStable)
            {
                __result *= 0.25f;
            }
        }
    }

}
