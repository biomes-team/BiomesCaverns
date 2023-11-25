using System;
using BiomesCaverns.ModSettings;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace BiomesCaverns
{
	public class BiomesCaverns : Mod
	{
		public const string Id = "rimworld.biomes.caverns";
		public const string Name = "Biomes! Caverns";
		private static readonly Version Version = typeof(BiomesCaverns).Assembly.GetName().Version;

		public BiomesCaverns(ModContentPack content) : base(content)
		{
			new Harmony(Id).PatchAll();
			LongEventHandler.ExecuteWhenFinished(InitializeWhenLoadingFinished);
		}

		private void InitializeWhenLoadingFinished()
		{
			GetSettings<Settings>();
			Message("Initialized");
		}

		public static void Message(string message) => Verse.Log.Message(PrefixMessage(message));
		public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
		private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return SettingsWindow.SettingsCategory();
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			SettingsWindow.DoWindowContents(inRect);
			base.DoSettingsWindowContents(inRect);
		}

	}
}