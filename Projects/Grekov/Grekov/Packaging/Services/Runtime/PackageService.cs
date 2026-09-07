using Grekov.Packaging.Entities;
using Grekov.Packaging.Interfaces;
using Grekov.Packaging.Services.Discovery;
using Grekov.Packaging.Services.Loading;

namespace Grekov.Packaging.Services.Runtime;

internal sealed class PackageService : IPackageRuntime
{
	private readonly PackageServiceState _state;
	private readonly DiscoveryWorkflow _discovery;
	private readonly PackageLoadOrderBuilder _loadOrderBuilder;
	private readonly ApplyWorkflow _applyWorkflow;
	private readonly SnapshotStore _snapshots;

	public PackageService(
		PackageServiceState state,
		DiscoveryWorkflow discovery,
		PackageLoadOrderBuilder loadOrderBuilder,
		ApplyWorkflow applyWorkflow,
		SnapshotStore snapshots)
	{
		_state = state;
		_discovery = discovery;
		_loadOrderBuilder = loadOrderBuilder;
		_applyWorkflow = applyWorkflow;
		_snapshots = snapshots;
	}

	public IReadOnlyList<PackageInstance> Packages => _state.Packages;
	public IReadOnlyList<PackageInstance> LoadOrder => _state.LoadOrder;

	public void LoadAll()
	{
		var discovery = _discovery.Discover();
		var loadOrder = _loadOrderBuilder.BuildLoadOrder(discovery);

		_applyWorkflow.Apply(loadOrder);

		_state.Packages.Clear();
		_state.Packages.AddRange(discovery.Packages);
		_state.PackagesById.Clear();

		foreach (var package in discovery.Packages)
			_state.PackagesById[package.Id] = package;

		_state.LoadOrder = loadOrder;
		_snapshots.Capture(_state.Packages, _state.LoadOrder);
	}
}
