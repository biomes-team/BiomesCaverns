using System;
using HarmonyLib;
using Verse;

namespace BiomesCaverns
{
	[StaticConstructorOnStartup]
	public static class BiomesCaverns
	{
		public const string Id = "rimworld.biomes.caverns";
		public const string Name = "Biomes! Caverns";
		private static readonly Version Version = typeof(BiomesCaverns).Assembly.GetName().Version;

		static BiomesCaverns()
		{
			new Harmony(Id).PatchAll();
			Message("Initialized");
		}

		public static void Message(string message) => Verse.Log.Message(PrefixMessage(message));
		public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));
		private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
	}
}