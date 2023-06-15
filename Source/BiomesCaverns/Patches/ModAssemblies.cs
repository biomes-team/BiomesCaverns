using System.Linq;
using System.Reflection;
using Verse;

namespace BiomesCaverns.Patches
{
	public static class ModAssemblies
	{
		private static Assembly _vfei;

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
				if (pack.PackageId == "oskarpotocki.vfe.insectoid")
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
			}
		}

		/// <summary>
		/// Checks if the Vanilla Factions Expanded - Insectoid mod is present in the current game.
		/// </summary>
		/// <returns>Boolean with the loaded state of this mod.</returns>
		public static Assembly VanillaFactionsExpandedInsectoid()
		{
			CacheMods();
			return _vfei;
		}
	}
}