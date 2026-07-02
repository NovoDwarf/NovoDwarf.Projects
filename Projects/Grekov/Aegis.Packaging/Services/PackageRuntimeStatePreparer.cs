using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;

namespace Aegis.Packaging.Services;

internal static class PackageRuntimeStatePreparer
{
	public static void PrepareStates(IReadOnlyList<PackageInstance> packages, IReadOnlyList<PackageInstance> loadOrder)
	{
		var loadedIds = new HashSet<string>(loadOrder
				.Where(package => package is { Enabled: true, HasErrors: false })
				.Select(static package => package.Id),
			StringComparer.OrdinalIgnoreCase);

		foreach (var package in packages)
		{
			if (!package.Enabled)
			{
				package.State = PackageState.Skipped;
				continue;
			}

			if (package.HasErrors)
			{
				package.State = PackageState.Error;
				continue;
			}

			package.State = loadedIds.Contains(package.Id)
				? PackageState.Loaded
				: PackageState.Skipped;
		}
	}
}
