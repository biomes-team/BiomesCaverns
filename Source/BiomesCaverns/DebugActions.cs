using LudeonTK;
using RimWorld;
using Verse;

namespace BiomesCaverns
{
	public static class DebugActions
	{
		/// <summary>
		/// Make the map filthy.
		/// </summary>
		[DebugAction(BiomesCaverns.Name, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void SpawnFilth()
		{
			var filthDef = ThingDefOf.Filth_Slime;
			var map = Current.Game.CurrentMap;
			var range = new IntRange(-1, 3);

			foreach (var cell in map.AllCells)
			{
				var amount = range.RandomInRange;
				if (amount > 0)
				{
					DebugThingPlaceHelper.DebugSpawn(filthDef, cell, amount);
				}
			}
		}
	}
}