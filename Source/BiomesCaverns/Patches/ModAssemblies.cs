using System.Reflection;
using Verse;

namespace BiomesCaverns.Patches
{
	/// <summary>
	/// Mod compatibility utility class.
	/// </summary>
	public static class ModAssemblies
	{
		private static Assembly _viems; // Vanilla Ideology Expanded - Memes and Structures.
		private static Assembly _vpeh; // Vanilla Psycasts Expanded - Hemosage.

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
				switch (packageId)
				{
					case "vanillaexpanded.vmemese":
						_viems = pack.assemblies.loadedAssemblies[0];
						break;
					case "vanillaexpanded.vpe.hemosage":
						_vpeh = pack.assemblies.loadedAssemblies[0];
						break;
				}
			}
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

		/// <summary>
		/// Checks if the Vanilla Psycasts Expanded - Hemosage mod is present in the current game.
		/// </summary>
		/// <returns>Assembly of this mod.</returns>
		public static Assembly VanillaPsycastsExpandedHemosage()
		{
			CacheMods();
			return _vpeh;
		}
	}
}