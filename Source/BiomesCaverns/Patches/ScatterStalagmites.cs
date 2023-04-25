using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches
{
	// GenStep_ScatterGroup.CanSpawn uses an internal type to access the current map. This makes postfixing
	// it a complicated issue. Instead, we store a reference to the map when the parent method is called.
	[HarmonyPatch(typeof(GenStep_ScatterGroup), "CalculateScatterInformation")]
	internal static class GenStep_ScatterGroup_CalculateScatterInformation
	{
		public static Map currentMap;

		internal static void Prefix(IntVec3 loc, Map map)
		{
			currentMap = map;
		}
	}

	[HarmonyPatch]
	internal static class GenStep_ScatterGroup_CanSpawn
	{
		static MethodBase TargetMethod()
		{
			foreach (var method in typeof(GenStep_ScatterGroup).GetMethods((BindingFlags) (-1)))
			{
				if (method.Name.Contains("CanSpawn"))
				{
					return method;
				}
			}

			return null;
		}

		private static readonly HashSet<ThingDef> StalagmiteDefs = new HashSet<ThingDef>
		{
			BC_DefOf.BMT_NaturalBuildings_Stalagmites_Small,
			BC_DefOf.BMT_NaturalBuildings_Stalagmites_Medium,
			BC_DefOf.BMT_NaturalBuildings_Stalagmites_Large
		};

		internal static void Postfix(ref bool __result, ThingDef def, IntVec3 cell)
		{
			if (__result == false || !StalagmiteDefs.Contains(def))
			{
				return;
			}

			__result = cell.GetTerrain(GenStep_ScatterGroup_CalculateScatterInformation.currentMap).smoothedTerrain != null;
		}
	}
}