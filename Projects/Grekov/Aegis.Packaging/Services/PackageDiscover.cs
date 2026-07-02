using Aegis.Packaging.Core.Constants;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Interfaces;

namespace Aegis.Packaging.Services;

public sealed class PackageDiscover
{
	private readonly IPackageCatalog _catalog;
	private readonly IPackageLoadOrderStore _orderStore;

	public PackageDiscover(IPackageCatalog catalog, IPackageLoadOrderStore orderStore)
	{
		_catalog = catalog;
		_orderStore = orderStore;
	}

	public PackageDiscoveryResult Discover()
	{
		var packagesById = new Dictionary<string, PackageInstance>(StringComparer.OrdinalIgnoreCase);
		var packages = new List<PackageInstance>();

		foreach (var package in _catalog.DiscoverPackages())
		{
			package.ParseMeta();
			packages.Add(package);

			if (string.IsNullOrWhiteSpace(package.Id))
				continue;

			if (packagesById.TryGetValue(package.Id, out var existing))
			{
				package.AddIssue(
					PackageIssueSeverity.Error,
					PackageIssues.IdDuplicate,
					PackageIssueStrings.DuplicatePackageId(package.Id, existing.RootPath));

				existing.AddIssue(
					PackageIssueSeverity.Error,
					PackageIssues.IdDuplicate,
					PackageIssueStrings.DuplicatePackageId(existing.Id, package.RootPath));
				continue;
			}

			packagesById[package.Id] = package;
		}

		ApplyUserOrder(packages);
		packages.Sort(PackageSort.CompareForLoadOrder);

		return new PackageDiscoveryResult(packages, packagesById);
	}

	private void ApplyUserOrder(IEnumerable<PackageInstance> packages)
	{
		var map = _orderStore.BuildIndexMap();

		foreach (var p in packages)
			p.UserOrderIndex = map.GetValueOrDefault(p.Id, int.MaxValue);
	}
}