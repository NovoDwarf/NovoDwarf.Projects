using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Services;

namespace Aegis.Packaging.Services;

internal static class PackageAffectedSetBuilder
{
	public static HashSet<string> BuildFullAffectedSet(IReadOnlyDictionary<string, PackageInstance> currentPackages, IReadOnlyDictionary<string, PackageInstance> nextPackages)
	{
		var affected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		foreach (var id in currentPackages.Keys)
			affected.Add(id);

		foreach (var id in nextPackages.Keys)
			affected.Add(id);

		return affected;
	}

	public static HashSet<string> BuildAffectedSet(string rootPackageId, IReadOnlyDictionary<string, PackageInstance> currentPackages, IReadOnlyDictionary<string, PackageInstance> nextPackages)
	{
		var affected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var reverse = BuildReverseDependencyMap(currentPackages, nextPackages);
		var queue = new Queue<string>();

		affected.Add(rootPackageId);
		queue.Enqueue(rootPackageId);

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();

			if (!reverse.TryGetValue(current, out var dependents))
				continue;

			foreach (var dependent in dependents.Where(affected.Add))
				queue.Enqueue(dependent);
		}

		return affected;
	}
	

	private static Dictionary<string, HashSet<string>> BuildReverseDependencyMap(
		IReadOnlyDictionary<string, PackageInstance> currentPackages,
		IReadOnlyDictionary<string, PackageInstance> nextPackages)
	{
		var map = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		foreach (var package in currentPackages.Values.Concat(nextPackages.Values))
		{
			foreach (var dependency in package.Dependencies)
				AddReverse(map, dependency.PackageId, package.Id);

			foreach (var dependencyId in package.Meta.OptionalDependenciesRaw.Select(PackageDependencyParser.TryGetPackageId).OfType<string>())
				AddReverse(map, dependencyId, package.Id);

			foreach (var dependencyId in package.Meta.LoadAfter.Select(static id => id?.Trim()).OfType<string>())
				AddReverse(map, dependencyId, package.Id);

			foreach (var dependencyId in package.Meta.LoadBefore.Select(static id => id?.Trim()).OfType<string>())
				AddReverse(map, package.Id, dependencyId);

			foreach (var packageId in package.Meta.IncompatiblePackages.Select(static id => id?.Trim()).OfType<string>())
			{
				AddReverse(map, packageId, package.Id);
				AddReverse(map, package.Id, packageId);
			}
		}

		return map;
	}

	private static void AddReverse(IDictionary<string, HashSet<string>> map, string dependencyId, string dependentId)
	{
		if (string.IsNullOrWhiteSpace(dependencyId) || string.IsNullOrWhiteSpace(dependentId))
			return;

		if (!map.TryGetValue(dependencyId, out var set))
		{
			set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			map[dependencyId] = set;
		}

		set.Add(dependentId);
	}
}
