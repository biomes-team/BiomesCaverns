using System.Collections.Generic;
using System.Xml;
using RimWorld;
using UnityEngine;
using Verse;

namespace BMT
{
    class GenStep_ScatterCrystals : GenStep_ScatterGroup
	{
		protected virtual bool ShouldSkipMapCrystal(Map map)
		{
			if (map.Biome.defName.Contains("Crystal"))
			{
				return false;
			}
			return true;
		}
		public override void Generate(Map map, GenStepParams parms)
		{
			if (ShouldSkipMapCrystal(map))
			{
				return;
			}
			else if (ShouldSkipMap(map))
			{
				return;
			}
			int num = CalculateFinalCount(map);
			for (int i = 0; i < num; i++)
			{
				if (!TryFindScatterCell(map, out var result))
				{
					return;
				}
				ScatterAt(result, map, parms);
				usedSpots.Add(result);
			}
			usedSpots.Clear();
		}
	}
	class GenStep_ScatterRedCrystals : GenStep_ScatterGroup
	{
		protected virtual bool ShouldSkipMapCrystal(Map map)
		{
			if (map.Biome.defName.Contains("BMT_EarthenDepths"))
			{
				return false;
			}
			return true;
		}
		public override void Generate(Map map, GenStepParams parms)
		{
			if (ShouldSkipMapCrystal(map))
			{
				return;
			}
			else if (ShouldSkipMap(map))
			{
				return;
			}
			int num = CalculateFinalCount(map);
			for (int i = 0; i < num; i++)
			{
				if (!TryFindScatterCell(map, out var result))
				{
					return;
				}
				ScatterAt(result, map, parms);
				usedSpots.Add(result);
			}
			usedSpots.Clear();
		}
	}
}
