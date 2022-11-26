using System.Collections.Generic;
using System.Xml;
using RimWorld;
using UnityEngine;
using Verse;

namespace BMT
{
    class GenStep_ScatterStalagmite : GenStep_ScatterGroup
	{
		protected virtual bool ShouldSkipMapStalagmite(Map map)
		{
			if (map.Biome.defName.Contains("BMT_ShallowCave") || map.Biome.defName.Contains("BMT_FungalForest") || map.Biome.defName.Contains("BMT_CrystalCaverns") || map.Biome.defName.Contains("BMT_EarthenDepths"))
			{
				return false;
			}
			return true;
		}
		public override void Generate(Map map, GenStepParams parms)
		{
			if (ShouldSkipMapStalagmite(map))
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
