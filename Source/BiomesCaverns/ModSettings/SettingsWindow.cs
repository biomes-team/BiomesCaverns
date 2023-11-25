using UnityEngine;
using Verse;

namespace BiomesCaverns.ModSettings
{
	/// <summary>
	/// Implementation of the mod settings window.
	/// </summary>
	public static class SettingsWindow
	{
		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public static string SettingsCategory()
		{
			return BiomesCaverns.Name;
		}

		/// <summary>
		/// Contents in the main part of the window.
		/// </summary>
		/// <param name="inRect"></param>
		public static void WindowContents(Rect inRect)
		{
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.CheckboxLabeled("BiomesCaverns_CoolEnclosedThickRoofAreasLabel".Translate(),
				ref Settings.Values.CoolEnclosedThickRoofAreas,
				"BiomesCaverns_CoolEnclosedThickRoofAreasHover".Translate());

			listing.End();
		}

		/// <summary>
		/// Draw additional buttons on the bottom button bar of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		private static void DrawBottomButtons(Rect inRect)
		{
			float resetX = inRect.width - Window.CloseButSize.x;
			// Dialog_ModSettings leaves a margin of Window.CloseButSize.y at the bottom for the close button.
			// Then, there are three pixels between the top border of the close button and the rest of this window.
			float resetY = inRect.height + Window.CloseButSize.y + 3;
			Rect resetButtonArea = new Rect(resetX, resetY, Window.CloseButSize.x, Window.CloseButSize.y);

			if (Widgets.ButtonText(resetButtonArea, "BiomesCaverns_ResetSettingsLabel".Translate()))
			{
				Settings.Reset();
			}

			TooltipHandler.TipRegion(resetButtonArea, "BiomesCaverns_ResetSettingsHover".Translate());
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public static void DoWindowContents(Rect inRect)
		{
			WindowContents(inRect);
			DrawBottomButtons(inRect);
		}
	}
}