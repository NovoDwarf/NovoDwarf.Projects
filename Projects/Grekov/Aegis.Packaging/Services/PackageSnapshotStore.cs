using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Entities;

namespace Aegis.Packaging.Services;

internal sealed class PackageSnapshotStore
{
	private readonly Dictionary<string, PackageRuntimeSnapshot> _lastSnapshotByPackage = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, string> _lastFingerprintByPackage = new(StringComparer.OrdinalIgnoreCase);
	
	private List<string> _lastDiscoveredOrder = [];
	private List<string> _lastLoadOrderIds = [];
	
	private bool _hasSnapshot;

	public bool CanReusePrevious(PackageDiscoveryResult discovery, IReadOnlyDictionary<string, string> fingerprints)
	{
		if (!_hasSnapshot)
			return false;

		var discoveredOrder = discovery.Packages
			.Select(static package => package.Id)
			.ToList();

		if (!_lastDiscoveredOrder.SequenceEqual(discoveredOrder, StringComparer.OrdinalIgnoreCase))
			return false;

		foreach (var package in discovery.Packages)
		{
			if (!_lastSnapshotByPackage.TryGetValue(package.Id, out var snapshot))
				return false;

			if (!fingerprints.TryGetValue(package.Id, out var fingerprint))
				return false;

			if (!_lastFingerprintByPackage.TryGetValue(package.Id, out var previousFingerprint) ||
			    !string.Equals(previousFingerprint, fingerprint, StringComparison.OrdinalIgnoreCase))
				return false;

			if (snapshot.Enabled != package.Enabled)
				return false;
		}

		return true;
	}

	public void ApplySnapshot(PackageDiscoveryResult discovery, PackageServiceState state)
	{
		state.PackagesById.Clear();
		state.Packages.Clear();

		foreach (var package in discovery.Packages)
		{
			if (!_lastSnapshotByPackage.TryGetValue(package.Id, out var snapshot))
				continue;

			package.Enabled = snapshot.Enabled;
			package.ClearIssues();
			package.Issues.AddRange(snapshot.Issues);
			package.State = snapshot.State;

			state.Packages.Add(package);
			state.PackagesById[package.Id] = package;
		}

		state.LoadOrder = _lastLoadOrderIds
			.Select(id => discovery.PackagesById.GetValueOrDefault(id))
			.OfType<PackageInstance>()
			.ToList();
	}

	public void CaptureSnapshot(
		PackageDiscoveryResult discovery,
		IReadOnlyDictionary<string, string> fingerprints,
		IReadOnlyList<PackageInstance> loadOrder)
	{
		_lastSnapshotByPackage.Clear();
		_lastFingerprintByPackage.Clear();

		foreach (var package in discovery.Packages)
		{
			if (string.IsNullOrWhiteSpace(package.Id))
				continue;

			_lastSnapshotByPackage[package.Id] = new PackageRuntimeSnapshot(package.Enabled, package.State, package.Issues.ToList());

			if (fingerprints.TryGetValue(package.Id, out var fingerprint))
				_lastFingerprintByPackage[package.Id] = fingerprint;
		}

		_lastDiscoveredOrder = discovery.Packages
			.Select(static package => package.Id)
			.ToList();

		_lastLoadOrderIds = loadOrder
			.Select(static package => package.Id)
			.ToList();

		_hasSnapshot = true;
	}

	private readonly record struct PackageRuntimeSnapshot(bool Enabled, PackageState State, List<PackageIssue> Issues);
}
