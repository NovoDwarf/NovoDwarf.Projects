using System.Diagnostics;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Interfaces;
using Aegis.Packaging.Core.Services;
using Aegis.Packaging.Entities;
using Aegis.Packaging.Extensions;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Services;

public sealed class PackageService : IPackageRuntime
{
	private readonly ILogger<PackageService> _logger;
	private readonly PackageLoadExecutionService _loadExecution;
	private readonly PackageConflictRegistry _packageConflicts;

	private readonly PackageServiceState _state = new();
	private readonly PackageDiscoveryWorkflow _discovery;
	private readonly PackageSnapshotStore _snapshots = new();
	private readonly PackageApplyWorkflow _apply;

	public PackageService(
		ILogger<PackageService> logger,
		PackageDiscover discovery,
		IPackageFingerprintProvider fingerprints,
		PackageValidator validation,
		PackageLoadExecutionService loadExecution,
		PackageConflictRegistry packageConflicts)
	{
		_logger = logger;
		_loadExecution = loadExecution;
		_packageConflicts = packageConflicts;

		_discovery = new PackageDiscoveryWorkflow(discovery, fingerprints, validation, _state);
		_apply = new PackageApplyWorkflow(_state, _loadExecution, _packageConflicts, _snapshots);
	}

	public IReadOnlyList<PackageInstance> Packages => _state.Packages;
	public IReadOnlyList<PackageInstance> LoadOrder => _state.LoadOrder;
	public IReadOnlyList<PackageConflictEvent> Conflicts => _packageConflicts.Events;

	public event Action? Changed;

	public void LoadAll() => ReloadAll();

	public void ReloadAll()
	{
		using var scope = _logger.BeginScope(new Dictionary<string, object?>
		{
			["Operation"] = "package.reload",
			["Mode"] = "all",
			["PackageId"] = "*"
		});

		_logger.PackageReloadStarted("all", "*", _state.Packages.Count);
		var discovery = _discovery.PrepareDiscovery();

		if (ExecuteReload("all", discovery, (current, d) =>
				PackageAffectedSetBuilder.BuildFullAffectedSet(current, d.PackagesById)))
			Changed?.Invoke();
	}

	public bool ReloadPackage(string packageId)
	{
		if (string.IsNullOrWhiteSpace(packageId))
			return false;

		var normalizedPackageId = packageId.Trim();
		using var scope = _logger.BeginScope(new Dictionary<string, object?>
		{
			["Operation"] = "package.reload",
			["Mode"] = "single",
			["PackageId"] = normalizedPackageId
		});

		_logger.PackageReloadStarted("single", normalizedPackageId, _state.Packages.Count);
		var discovery = _discovery.PrepareDiscovery();

		if (!_state.PackagesById.ContainsKey(normalizedPackageId) && !discovery.PackagesById.ContainsKey(normalizedPackageId))
			return false;

		return ExecuteReload("single", discovery, (current, d) =>
			PackageAffectedSetBuilder.BuildAffectedSet(normalizedPackageId, current, d.PackagesById));
	}

	public bool SetEnabled(string packageId, bool enabled)
	{
		if (!_state.PackagesById.TryGetValue(packageId, out var pkg))
			return false;

		if (pkg.Enabled == enabled)
			return true;

		using var scope = _logger.BeginScope(new Dictionary<string, object?>
		{
			["Operation"] = "package.set-enabled",
			["PackageId"] = packageId,
			["Enabled"] = enabled
		});

		_logger.PackageEnabledChanged(packageId, enabled);

		var hadOverride = _state.EnabledOverrides.TryGetValue(packageId, out var previousOverride);
		_state.EnabledOverrides[packageId] = enabled;

		var reloaded = ReloadPackage(packageId);

		if (reloaded)
			return true;

		if (hadOverride)
			_state.EnabledOverrides[packageId] = previousOverride;
		else
			_state.EnabledOverrides.Remove(packageId);

		return false;
	}

	private bool ExecuteReload(
		string logMode,
		PackageDiscoveryResult discovery,
		Func<IReadOnlyDictionary<string, PackageInstance>, PackageDiscoveryResult, HashSet<string>> buildAffectedSet)
	{
		var sw = Stopwatch.StartNew();
		var currentPackages = _state.Packages.ToDictionary(package => package.Id, StringComparer.OrdinalIgnoreCase);
		var currentLoadOrder = _state.LoadOrder.ToList();
		var fingerprints = _discovery.BuildFingerprintMap(discovery.Packages);

		if (_snapshots.CanReusePrevious(discovery, fingerprints))
		{
			_snapshots.ApplySnapshot(discovery, _state);
			_logger.SkipReload();
			return true;
		}

		var nextLoadOrder = _loadExecution.BuildLoadOrder(discovery);
		var affectedIds = buildAffectedSet(currentPackages, discovery);

		if (affectedIds.Count == 0)
			return false;

		if (!_apply.TryApplyDiscovery(discovery, nextLoadOrder, currentLoadOrder, currentPackages, affectedIds, fingerprints))
			return false;

		sw.Stop();
		_logger.PackageReloadCompleted(
			logMode,
			_state.Packages.Count(package => package.State == PackageState.Loaded),
			_state.Packages.Count,
			sw.ElapsedMilliseconds);

		return true;
	}
}
