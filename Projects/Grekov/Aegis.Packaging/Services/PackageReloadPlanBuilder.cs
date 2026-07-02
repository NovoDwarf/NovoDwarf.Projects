using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Entities;

namespace Aegis.Packaging.Services;

internal sealed class PackageReloadPlanBuilder
{
	private readonly PackageLoadExecutionService _loadExecution;
	private readonly PackageSnapshotStore _snapshots;

	public PackageReloadPlanBuilder(PackageLoadExecutionService loadExecution, PackageSnapshotStore snapshots)
	{
		_loadExecution = loadExecution;
		_snapshots = snapshots;
	}

	public PackageReloadPlan BuildFullReload(
		PackageDiscoveryResult discovery,
		IReadOnlyDictionary<string, PackageInstance> currentPackages,
		IReadOnlyList<PackageInstance> currentLoadOrder,
		IReadOnlyDictionary<string, string> fingerprints)
	{
		var canReuseSnapshot = _snapshots.CanReusePrevious(discovery, fingerprints);
		var nextLoadOrder = canReuseSnapshot
			? currentLoadOrder
			: _loadExecution.BuildLoadOrder(discovery);

		return new PackageReloadPlan(
			"all",
			"*",
			discovery,
			nextLoadOrder,
			currentLoadOrder,
			currentPackages,
			PackageAffectedSetBuilder.BuildFullAffectedSet(currentPackages, discovery.PackagesById),
			fingerprints,
			canReuseSnapshot);
	}

	public PackageReloadPlan BuildPackageReload(
		string packageId,
		PackageDiscoveryResult discovery,
		IReadOnlyDictionary<string, PackageInstance> currentPackages,
		IReadOnlyList<PackageInstance> currentLoadOrder,
		IReadOnlyDictionary<string, string> fingerprints)
	{
		var nextLoadOrder = _loadExecution.BuildLoadOrder(discovery);
		return new PackageReloadPlan(
			"single",
			packageId,
			discovery,
			nextLoadOrder,
			currentLoadOrder,
			currentPackages,
			PackageAffectedSetBuilder.BuildAffectedSet(packageId, currentPackages, discovery.PackagesById),
			fingerprints,
			false);
	}
}
