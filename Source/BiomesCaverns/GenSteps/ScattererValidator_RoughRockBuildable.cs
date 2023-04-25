using Verse;

namespace BiomesCaverns.GenSteps
{
	public class ScattererValidator_RoughRockBuildable : ScattererValidator_Buildable
	{
		private static bool RadialAllows(IntVec3 center, Map map)
		{
			foreach (var cell in GenRadial.RadialCellsAround(center, 4.0F, true))
			{
				if (cell.GetTerrain(map).smoothedTerrain == null)
				{
					return false;
				}
			}

			return true;
		}

		public override bool Allows(IntVec3 c, Map map)
		{
			return base.Allows(c, map) && RadialAllows(c, map);
		}
	}
}