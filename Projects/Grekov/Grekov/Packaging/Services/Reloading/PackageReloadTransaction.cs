using System.Diagnostics;
using Grekov.Packaging.Entities;
using Grekov.Packaging.Extensions;
using Grekov.Packaging.Services.Conflicts;
using Grekov.Packaging.Services.Loading;
using Microsoft.Extensions.Logging;

namespace Grekov.Packaging.Services.Reloading;

internal sealed class PackageReloadTransaction
{
	private readonly PackageLoaderPipeline _loaderPipeline;
	private readonly ILogger<PackageReloadTransaction> _logger;

	public PackageReloadTransaction(PackageLoaderPipeline loaderPipeline, ILogger<PackageReloadTransaction> logger)
	{
		_loaderPipeline = loaderPipeline;
		_logger = logger;
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

		_loaderPipeline.BeginTransaction();

		try
		{
			_loaderPipeline.UnloadBatch(previousAffected, packageConflicts);
			_loaderPipeline.LoadBatch(nextAffected, packageConflicts);

			_loaderPipeline.CommitTransaction();

			reloadStopwatch.Stop();
			
			_logger.PackageReloadCommitted(reloadStopwatch.ElapsedMilliseconds);
			
			return true;
		}
		catch (Exception ex)
		{
			_logger.PackageReloadTransactionFailed(ex);

			_loaderPipeline.RollbackTransaction();

			try
			{
				_loaderPipeline.UnloadBatch(nextAffected, packageConflicts);
				_loaderPipeline.LoadBatch(previousAffected, packageConflicts);
			}
			catch (Exception rollbackEx)
			{
				_logger.PackageReloadRollbackFailed(rollbackEx);

				foreach (var package in previousAffected)
					package.AddIssue(PackageIssues.RollbackFailureIssue(rollbackEx.Message));
			}

			return false;
		}
	}
}
