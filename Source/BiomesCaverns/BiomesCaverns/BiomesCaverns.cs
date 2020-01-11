using Harmony;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace BiomesCaverns
{
    [StaticConstructorOnStartup]
    public static class BiomesCaverns
    {
        public const string Id = "rimworld.biomes.caverns";
        public const string Name = "Biomes! Caverns";
        public static string Version = (Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute).InformationalVersion;

        static BiomesCaverns()
        {
            //HarmonyInstance.Create(Id).PatchAll();
            Log("Initialized");
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}
