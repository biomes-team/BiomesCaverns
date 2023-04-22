using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace BiomesCaverns.Patches
{
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
			/*
			if (__result == false || !StalagmiteDefs.Contains(def))
			{
				return;
			}

			__result = cell.GetTerrain(__instance.map).smoothedTerrain != null;
			*/
		}
	}
}