using BiomesCore;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace BiomesCaverns
{
	/// <summary>
	/// Transpiler helpers may replace vanilla code with the functions in this class.
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// Cavern aware version of the vanilla check for seeing if a room should be considered to be outside.
		/// </summary>
		/// <param name="room">Room being checked.</param>
		/// <param name="cell">Cell being checked.</param>
		/// <returns>True if the room should be considered to be outdoors.</returns>
		public static bool CavernAwarePsychologicallyOutdoors(Room room, IntVec3 cell)
		{
			if (cell.GetRoof(room.Map) == BiomesCoreDefOf.BMT_RockRoofStable)
			{
				// Vanilla PsychologicallyOutdoors uses the number of open roof cells to calculate if the room should be
				// considered to be outdoors. In a cave, we can assume that all cells will have stable rock roof.
				// So in order to see if a room should be considered to be outdoors, the same vanilla threshold is checked
				// against the total cell count of the room.
				return room.TouchesMapEdge || room.CellCount > 300;
			}

			return room.PsychologicallyOutdoors;
		}

		/// <summary>
		/// In certain cases, cavern roof needs to be ignored, such as when landing royalty shuttles or deciding if
		/// something can physically drop into the cell.
		/// </summary>
		/// <param name="cell">Cell to be checked</param>
		/// <param name="map">Map of the cell.</param>
		/// <returns>Roof of the cell. Return null if the roof is a cavern roof, or if there is no roof.</returns>
		public static RoofDef CellGetNonCavernRoof(IntVec3 cell, Map map)
		{
			RoofDef roofDef = cell.GetRoof(map);
			return roofDef != BiomesCoreDefOf.BMT_RockRoofStable ? roofDef : null;
		}

		/// <summary>
		/// Check if the cell has a roof that is not cavern roof.
		/// </summary>
		/// <param name="cell">Cell to check.</param>
		/// <param name="map">Map of this cell.</param>
		/// <returns>True if the cell has any kind of roof, except for cavern stable rock roof.</returns>
		public static bool CellHasNonCavernRoof(IntVec3 cell, Map map)
		{
			return CellGetNonCavernRoof(cell, map) != null;
		}

		public static bool RoofGridHasNonCavernRoof(RoofGrid roofGrid, IntVec3 cell)
		{
			RoofDef roofDef = roofGrid.RoofAt(cell);
			return roofDef != null && roofDef != BiomesCoreDefOf.BMT_RockRoofStable;
		}

		public static RoofDef GetRoofThickIfCavern(IntVec3 cell, Map map)
		{
			RoofDef roofDef = cell.GetRoof(map);
			return roofDef == BiomesCoreDefOf.BMT_RockRoofStable ? RoofDefOf.RoofRockThick : roofDef;
		}

		/// <summary>
		/// In certain cases, UsesOutdoorTemperature is used to determine if a pawn is not in an underground location. In
		/// the case of caves, undergrounders should be considered underground even when they are in cells that can use
		/// outdoor temperature.
		/// </summary>
		/// <param name="cell">Cell to be checked</param>
		/// <param name="map">Map of the cell.</param>
		/// <returns>True if the cell is not part of a cavern, and also uses outdoor temperature.</returns>
		public static bool NotCavernAndUsesOutdoorTemperature(IntVec3 cell, Map map)
		{
			return cell.GetRoof(map) != BiomesCoreDefOf.BMT_RockRoofStable && cell.UsesOutdoorTemperature(map);
		}

		public static bool IsCavern_Direct(Map map)
		{
			return map?.Biome?.GetModExtension<BiomesCore.DefModExtensions.BiomesMap>()?.isCavern == true;
		}

		private static List<Map> cachedMapsCaverns = [];
		private static List<Map> cachedMapsNonCaverns = [];

		public static bool IsCavern(this Map map)
		{
			if (cachedMapsNonCaverns.Contains(map))
			{
				return false;
			}
			if (cachedMapsCaverns.Contains(map))
			{
				return true;
			}
			if (IsCavern_Direct(map))
			{
				cachedMapsCaverns.Add(map);
				return true;
			}
			else
			{
				cachedMapsNonCaverns.Add(map);
			}
			return false;
		}

		public static void CavernPlantSpawnerTick(Map map)
		{
			WildPlantSpawner wildPlantSpawner = map.wildPlantSpawner;
			int area = map.Area;
			int num = Mathf.CeilToInt((float)area * 0.0001f);
			float currentPlantDensityFactor = wildPlantSpawner.CurrentPlantDensityFactor;
			wildPlantSpawner.CacheWholeMapNumDesiredPlants();
			int num2 = Mathf.CeilToInt(10000f);
			float cachedChanceFromDensity = wildPlantSpawner.CachedChanceFromDensity;
			for (int i = 0; i < num; i++)
			{
				if (wildPlantSpawner.cycleIndex >= area)
				{
					wildPlantSpawner.calculatedWholeMapNumDesiredPlants = wildPlantSpawner.calculatedWholeMapNumDesiredPlantsTmp;
					wildPlantSpawner.calculatedWholeMapNumDesiredPlantsTmp = 0f;
					wildPlantSpawner.calculatedWholeMapNumNonZeroFertilityCells = wildPlantSpawner.calculatedWholeMapNumNonZeroFertilityCellsTmp;
					wildPlantSpawner.calculatedWholeMapNumNonZeroFertilityCellsTmp = 0;
					wildPlantSpawner.cycleIndex = 0;
				}
				IntVec3 intVec = map.cellsInRandomOrder.Get(wildPlantSpawner.cycleIndex);
				wildPlantSpawner.calculatedWholeMapNumDesiredPlantsTmp += wildPlantSpawner.GetDesiredPlantsCountAt(intVec, currentPlantDensityFactor);
				if (map.fertilityGrid.FertilityAt(intVec) > 0f)
				{
					wildPlantSpawner.calculatedWholeMapNumNonZeroFertilityCellsTmp++;
				}
				float mtb = map.BiomeAt(intVec).wildPlantRegrowDays;
				float num3 = wildPlantSpawner.OverrideDensityForFertilityCurve.Evaluate(map.fertilityGrid.FertilityAt(intVec));
				if (Rand.Chance((num3 > 0f) ? num3 : cachedChanceFromDensity) && Rand.MTBEventOccurs(mtb, 60000f, num2) && wildPlantSpawner.CanRegrowAt(intVec))
				{
					wildPlantSpawner.CheckSpawnWildPlantAt(intVec, currentPlantDensityFactor, wildPlantSpawner.calculatedWholeMapNumDesiredPlants);
				}
				wildPlantSpawner.cycleIndex++;
			}
		}

	}
}