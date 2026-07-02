using Aegis.Packaging.Core.Constants;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;

namespace Aegis.Packaging.Services;

public static class PackageGraph
{
	public static IReadOnlyList<PackageInstance> TopologicalSort(
		IReadOnlyCollection<PackageInstance> packages,
		Func<string, PackageInstance?> resolveById,
		Action<PackageInstance, PackageIssue>? addIssue)
	{
		var outgoing = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
		var indegree = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		foreach (var p in packages)
		{
			indegree.TryAdd(p.Id, 0);
			outgoing.TryAdd(p.Id, []);
		}

		foreach (var p in packages)
		foreach (var dep in p.Dependencies)
		{
			var depPkg = resolveById(dep.PackageId);

			if (depPkg == null || !outgoing.ContainsKey(depPkg.Id))
			{
				addIssue?.Invoke(
					p,
					new PackageIssue(PackageIssueSeverity.Error, PackageIssues.DependencyMissing,
						PackageIssueStrings.DependencyIsMissing(dep.PackageId)));
				continue;
			}

			outgoing[depPkg.Id].Add(p.Id);
			indegree[p.Id] = indegree.GetValueOrDefault(p.Id) + 1;
		}

		foreach (var p in packages)
		{
			foreach (var loadAfterId in p.Meta.LoadAfter.Select(static id => id?.Trim()).Where(static id => !string.IsNullOrWhiteSpace(id)))
			{
				var before = resolveById(loadAfterId!);
				if (before == null || !outgoing.ContainsKey(before.Id))
					continue;

				outgoing[before.Id].Add(p.Id);
				indegree[p.Id] = indegree.GetValueOrDefault(p.Id) + 1;
			}

			foreach (var loadBeforeId in p.Meta.LoadBefore.Select(static id => id?.Trim()).Where(static id => !string.IsNullOrWhiteSpace(id)))
			{
				var after = resolveById(loadBeforeId!);
				if (after == null || !outgoing.ContainsKey(after.Id))
					continue;

				outgoing[p.Id].Add(after.Id);
				indegree[after.Id] = indegree.GetValueOrDefault(after.Id) + 1;
			}
		}

		var comparer = Comparer<PackageInstance>.Create(PackageSort.CompareForLoadOrder);
		var ready = new SortedSet<PackageInstance>(comparer);

		foreach (var p in packages)
			if (indegree.GetValueOrDefault(p.Id) == 0)
				ready.Add(p);

		var result = new List<PackageInstance>(packages.Count);
		while (ready.Count > 0)
		{
			var next = ready.Min!;
			ready.Remove(next);
			result.Add(next);

			if (!outgoing.TryGetValue(next.Id, out var deps))
				continue;

			foreach (var dependentId in deps)
			{
				indegree[dependentId] = indegree.GetValueOrDefault(dependentId) - 1;
				if (indegree[dependentId] != 0)
					continue;

				var pkg = resolveById(dependentId);
				if (pkg != null)
					ready.Add(pkg);
			}
		}

		if (result.Count == packages.Count)
			return result;

		var resolvedIds = new HashSet<string>(result.Select(static r => r.Id), StringComparer.OrdinalIgnoreCase);
		var unresolved = packages
			.Where(p => !resolvedIds.Contains(p.Id))
			.ToList();

		foreach (var p in unresolved)
			addIssue?.Invoke(
				p,
				new PackageIssue(PackageIssueSeverity.Error, PackageIssues.DependencyCycle,
					PackageIssueStrings.DependencyCycleDetected()));

		unresolved.Sort(PackageSort.CompareForLoadOrder);
		result.AddRange(unresolved);

		return result;
	}
}
