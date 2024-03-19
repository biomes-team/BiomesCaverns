using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BiomesCaverns
{
    [StaticConstructorOnStartup]
    internal static class DependencyVersionCheck
    {
        private const string OwnName = "Biomes! Caverns";
        
        private static readonly List<Dependency> _dependencies = new List<Dependency>
        {
            new Dependency("Biomes! Core", "BiomesCore", new Version("2.6.0.0")),
            new Dependency("Geological Landforms", "GeologicalLandforms", new Version("1.6.8.0"))
        };

        static DependencyVersionCheck()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var dep in _dependencies)
            {
                var assembly = assemblies.FirstOrDefault(a => a.GetName().Name == dep.AssemblyName);

                if (assembly == null)
                {
                    Log.Error($"[{OwnName}] Required dependency is missing: {dep.DisplayName}");
                }
                else if (assembly.GetName().Version < dep.MinVersion)
                {
                    Log.Error($"[{OwnName}] You have an outdated version {assembly.GetName().Version} of {dep.DisplayName} " +
                              $"but version {dep.MinVersion} or newer is required! This will cause errors! " +
                              $"If you are using Steam, unsubscribe and then resubscribe {dep.DisplayName} to force an update.");
                }
            }
        }

        private struct Dependency
        {
            public readonly string DisplayName;
            public readonly string AssemblyName;
            public readonly Version MinVersion;

            public Dependency(string displayName, string assemblyName, Version minVersion)
            {
                DisplayName = displayName;
                AssemblyName = assemblyName;
                MinVersion = minVersion;
            }
        }
    }
}
