using System.Collections.Generic;
using Verse;

namespace BiomesCaverns.ModSettings
{
	/// <summary>
	/// Contains data for all settings values. The default values of this object are the initial default settings for the mod.
	/// </summary>
	public class SettingValues
	{
		/// <summary>
		/// By default, the game will cool enclosed areas under thick mountain roof. You can disable this setting to make
		/// the Earthen Depths ignore this feature, making the biome even more challenging.
		/// </summary>
		public bool AllowCoolingEnclosedEarthenDepthsAreas = true;
	}

	/// <summary>
	/// Allows the rest of the mod to access a SettingValues instance. Handles resetting, save and load.
	/// </summary>
	public class Settings : Verse.ModSettings
	{
		/// <summary>
		/// Single instance of the setting values of this mod. Uses static for performance reasons.
		/// </summary>
		public static SettingValues Values = new SettingValues();

		/// <summary>
		/// Set all settings to their default values.
		/// </summary>
		public static void Reset()
		{
			Values = new SettingValues();
		}

		/// <summary>
		/// Save and load preferences.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref Values.AllowCoolingEnclosedEarthenDepthsAreas,
				nameof(Values.AllowCoolingEnclosedEarthenDepthsAreas), defaultValue: true);
		}
	}
}