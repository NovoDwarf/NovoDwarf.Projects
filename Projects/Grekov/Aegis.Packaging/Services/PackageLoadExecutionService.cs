using System.Diagnostics;
using Aegis.Packaging.Core.Constants;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Interfaces;
using Aegis.Packaging.Core.Services;
using Aegis.Packaging.Extensions;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Services;

public sealed class PackageLoadExecutionService
{
	private readonly IEnumerable<IPackageContentLoader> _contentLoaders;
	private readonly ILogger<PackageLoadExecutionService> _logger;

	public PackageLoadExecutionService(IEnumerable<IPackageContentLoader> contentLoaders, ILogger<PackageLoadExecutionService> logger)
	{
		_contentLoaders = contentLoaders
			.OrderBy(static loader => loader.Stage)
			.ThenBy(static loader => loader.Order)
			.ToArray();
		
		_logger = logger;
	}

	public IReadOnlyList<PackageInstance> BuildLoadOrder(PackageDiscoveryResult discovery)
	{
		var candidates = discovery.Packages
			.Where(package => package.Enabled && !package.Issues.Any(issue => PackageIssues.BlocksLoading(issue.Code)))
			.ToList();

		var ordered = PackageGraph.TopologicalSort(
			candidates,
			id => discovery.PackagesById.GetValueOrDefault(id),
			(p, issue) => p.Issues.Add(issue));

		_logger.PackageLoadOrderBuilt(candidates.Count, ordered.Count);
		return ordered;
	}

	public bool ApplyReload(
		IReadOnlyList<PackageInstance> previousLoadOrder,
		IReadOnlyList<PackageInstance> nextLoadOrder,
		IReadOnlySet<string> affectedPackageIds,
		PackageConflictRegistry packageConflicts)
	{
		var previousAffected = previousLoadOrder
			.Where(package => affectedPackageIds.Contains(package.Id))
			.ToList();
		
		var nextAffected = nextLoadOrder
			.Where(package => affectedPackageIds.Contains(package.Id))
			.ToList();
		
		var reloadStopwatch = Stopwatch.StartNew();

		using var scope = _logger.BeginScope(new Dictionary<string, object?>
		{
			["Operation"] = "package.reload.apply",
			["AffectedCount"] = affectedPackageIds.Count
		});

		_logger.PackageReloadApplyStarted(previousAffected.Count, nextAffected.Count);

		foreach (var loader in _contentLoaders)
			loader.BeginTransaction();

		try
		{
			UnloadBatch(previousAffected, packageConflicts);
			LoadBatch(nextAffected, packageConflicts);

			foreach (var loader in _contentLoaders)
				loader.CommitTransaction();

			reloadStopwatch.Stop();
			_logger.PackageReloadCommitted(reloadStopwatch.ElapsedMilliseconds);
			return true;
		}
		catch (Exception ex)
		{
			_logger.PackageReloadTransactionFailed(ex);

			foreach (var loader in _contentLoaders)
				loader.RollbackTransaction();

			try
			{
				UnloadBatch(nextAffected, packageConflicts);
				LoadBatch(previousAffected, packageConflicts);
			}
			catch (Exception rollbackEx)
			{
				_logger.PackageReloadRollbackFailed(rollbackEx);

				foreach (var package in previousAffected)
					package.AddIssue(
						PackageIssueSeverity.Error,
						PackageIssues.RollbackFailure,
						PackageIssueStrings.RollbackFailed(rollbackEx.Message));
			}

			return false;
		}
	}

	public void ClearAll(PackageConflictRegistry packageConflicts)
	{
		foreach (var loader in _contentLoaders)
			loader.Clear();

		packageConflicts.Clear();
	}

	private void UnloadBatch(IEnumerable<PackageInstance> packages, PackageConflictRegistry packageConflicts)
	{
		foreach (var package in packages.Reverse())
		{
			foreach (var loader in _contentLoaders.Reverse())
			{
				using var scope = _logger.BeginScope(new Dictionary<string, object?>
				{
					["Operation"] = "package.unload",
					["PackageId"] = package.Id,
					["Loader"] = loader.GetType().Name,
					["Stage"] = loader.Stage.ToString()
				});

				loader.UnloadPackage(package.Id);
				_logger.PackageLoaderUnloaded(loader.GetType().Name, package.Id, loader.Stage.ToString());
			}

			packageConflicts.UnregisterPackage(package.Id);
		}
	}

	private void LoadBatch(IEnumerable<PackageInstance> packages, PackageConflictRegistry packageConflicts)
	{
		foreach (var package in packages)
		{
			if (!package.Enabled || package.HasErrors)
				continue;

			foreach (var loader in _contentLoaders)
			{
				using var scope = _logger.BeginScope(new Dictionary<string, object?>
				{
					["Operation"] = "package.load",
					["PackageId"] = package.Id,
					["Loader"] = loader.GetType().Name,
					["Stage"] = loader.Stage.ToString()
				});
				var loaderStopwatch = Stopwatch.StartNew();

				try
				{
					_logger.PackageLoaderStarted(loader.GetType().Name, package.Id, loader.Stage.ToString());
					loader.LoadPackage(package, packageConflicts);
					loaderStopwatch.Stop();
					_logger.PackageLoaderCompleted(loader.GetType().Name, package.Id, loader.Stage.ToString(), loaderStopwatch.ElapsedMilliseconds);
				}
				catch (Exception ex)
				{
					package.AddIssue(
						PackageIssueSeverity.Error,
						PackageIssues.LoaderFailure,
						PackageIssueStrings.LoaderFailed(loader.GetType().Name, ex.Message));
					throw;
				}
			}
		}
	}
}
