using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Interfaces;
using Aegis.Packaging.Entities;

namespace Aegis.Packaging.Services;

internal sealed class PackageDiscoveryWorkflow
{
	private readonly PackageDiscover _discovery;
	private readonly IPackageFingerprintProvider _fingerprints;
	private readonly PackageValidator _validation;
	private readonly PackageServiceState _state;

	public PackageDiscoveryWorkflow(
		PackageDiscover discovery,
		IPackageFingerprintProvider fingerprints,
		PackageValidator validation,
		PackageServiceState state)
	{
		_discovery = discovery;
		_fingerprints = fingerprints;
		_validation = validation;
		_state = state;
	}

	public PackageDiscoveryResult PrepareDiscovery()
	{
		var discovery = _discovery.Discover();

		foreach (var package in discovery.Packages)
			if (_state.EnabledOverrides.TryGetValue(package.Id, out var enabled))
				package.Enabled = enabled;

		_validation.Validate(discovery);
		return discovery;
	}

	public Dictionary<string, string> BuildFingerprintMap(IEnumerable<PackageInstance> packages)
	{
		var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		foreach (var package in packages)
		{
			if (string.IsNullOrWhiteSpace(package.Id))
				continue;

			map[package.Id] = _fingerprints.ComputePackageFingerprint(package);
		}

		return map;
	}
}
