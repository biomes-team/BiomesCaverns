using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BiomesCaverns.DefModExtensions
{
	public class PlantCavernGraphic : DefModExtension
	{
		public string texPath;

		private Graphic graphic;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var err in base.ConfigErrors())
			{
				yield return err;
			}

			if (texPath == null)
			{
				Log.Error($"PlantCavernGraphic must define a texPath.");
			}
		}

		public Graphic GetGraphic()
		{
			if (graphic == null)
			{
				graphic = GraphicDatabase.Get(typeof(Graphic_Random), texPath, ShaderDatabase.CutoutPlant, Vector2.one,
					Color.white, Color.white);
			}

			return graphic;
		}
	}
}