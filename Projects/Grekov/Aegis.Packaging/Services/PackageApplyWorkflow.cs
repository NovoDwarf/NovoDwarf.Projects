using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Services;
using Aegis.Packaging.Entities;

namespace Aegis.Packaging.Services;

internal sealed class PackageApplyWorkflow
{
	private readonly PackageServiceState _state;
	private readonly PackageLoadExecutionService _loadExecution;
	private readonly PackageConflictRegistry _packageConflicts;
	private readonly PackageSnapshotStore _snapshots;

	public PackageApplyWorkflow(
		PackageServiceState state,
		PackageLoadExecutionService loadExecution,
		PackageConflictRegistry packageConflicts,
		PackageSnapshotStore snapshots)
	{
		_state = state;
		_loadExecution = loadExecution;
		_packageConflicts = packageConflicts;
		_snapshots = snapshots;
	}

	public bool TryApplyDiscovery(
		PackageDiscoveryResult discovery,
		IReadOnlyList<PackageInstance> nextLoadOrder,
		IReadOnlyList<PackageInstance> currentLoadOrder,
		IReadOnlyDictionary<string, PackageInstance> currentPackages,
		HashSet<string> affectedIds,
		IReadOnlyDictionary<string, string> fingerprints)
	{
		if (affectedIds.Count == 0)
			return false;

		var previousLoadOrder = currentLoadOrder
			.Where(package => affectedIds.Contains(package.Id))
			.ToList();

		var applied = _loadExecution.ApplyReload(previousLoadOrder, nextLoadOrder, affectedIds, _packageConflicts);
		if (!applied)
		{
			RestoreCurrentCatalog(currentPackages, currentLoadOrder);
			return false;
		}

		PackageRuntimeStatePreparer.PrepareStates(discovery.Packages, nextLoadOrder);
		_state.PackagesById.Clear();
		_state.Packages.Clear();

		foreach (var package in discovery.Packages)
		{
			_state.Packages.Add(package);
			_state.PackagesById[package.Id] = package;
		}

		_state.LoadOrder = nextLoadOrder;
		_snapshots.CaptureSnapshot(discovery, fingerprints, _state.LoadOrder);

		return true;
	}

	private void RestoreCurrentCatalog(
		IReadOnlyDictionary<string, PackageInstance> currentPackages,
		IReadOnlyList<PackageInstance> currentLoadOrder)
	{
		_state.PackagesById.Clear();
		_state.Packages.Clear();

		foreach (var package in currentPackages.Values.OrderBy(static package => package, Comparer<PackageInstance>.Create(PackageSort.CompareForLoadOrder)))
		{
			_state.Packages.Add(package);
			_state.PackagesById[package.Id] = package;
		}

		_state.LoadOrder = currentLoadOrder.ToList();
	}
}
