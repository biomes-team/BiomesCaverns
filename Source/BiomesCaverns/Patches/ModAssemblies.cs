using System.Linq;
using System.Reflection;
using Verse;

namespace BiomesCaverns.Patches
{
	public static class ModAssemblies
	{
		private static Assembly _vfei; // Vanilla Factions Expanded - Insectoids.
		private static Assembly _viems; // Vanilla Ideology Expanded - Memes and Structures;

		private static bool _cachedValues;

		private static void CacheMods()
		{
			if (_cachedValues)
			{
				return;
			}

			_cachedValues = true;
			foreach (var pack in LoadedModManager.RunningMods)
			{
				var packageId = pack.PackageId;
				if (packageId == "oskarpotocki.vfe.insectoid")
				{
					foreach (var assembly in pack.assemblies.loadedAssemblies)
					{
						if (assembly.FullName.Contains("VFEI"))
						{
							_vfei = assembly;
							break;
						}
					}
				}
				else if (packageId == "vanillaexpanded.vmemese")
				{
					_viems = pack.assemblies.loadedAssemblies[0];
				}
			}
		}

		/// <summary>
		/// Checks if the Vanilla Factions Expanded - Insectoid mod is present in the current game.
		/// </summary>
		/// <returns>Assembly of this mod.</returns>
		public static Assembly VanillaFactionsExpandedInsectoid()
		{
			CacheMods();
			return _vfei;
		}

		/// <summary>
		/// Checks if the Vanilla Ideology Expanded - Memes and Structures mod is present in the current game.
		/// </summary>
		/// <returns>Assembly of this mod.</returns>
		public static Assembly VanillaIdeologyExpandedMemesStructures()
		{
			CacheMods();
			return _viems;
		}
	}
}