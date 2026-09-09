using System.Diagnostics;
using Grekov.Packaging.Entities;
using Grekov.Packaging.Enums;
using Grekov.Packaging.Extensions;
using Grekov.Packaging.Interfaces;
using Grekov.Packaging.Services.Conflicts;
using Microsoft.Extensions.Logging;

namespace Grekov.Packaging.Services.Loading;

internal sealed class PackageLoaderPipeline
{
	private readonly IReadOnlyList<IPackageContentLoader> _contentLoaders;
	private readonly ILogger<PackageLoaderPipeline> _logger;

	public PackageLoaderPipeline(IEnumerable<IPackageContentLoader> contentLoaders, ILogger<PackageLoaderPipeline> logger)
	{
		_contentLoaders =
		[
			.. contentLoaders
			   .OrderBy(static loader => loader.Stage)
			   .ThenBy(static loader => loader.Order)
		];
		
		_logger = logger;
	}

	public void BeginTransaction()
	{
		foreach (var loader in _contentLoaders)
			loader.BeginTransaction();
	}

	public void CommitTransaction()
	{
		foreach (var loader in _contentLoaders)
			loader.CommitTransaction();
	}

	public void RollbackTransaction()
	{
		foreach (var loader in _contentLoaders)
			loader.RollbackTransaction();
	}

	public void ClearAll(PackageConflictRegistry packageConflicts)
	{
		foreach (var loader in _contentLoaders)
			loader.Clear();

		packageConflicts.Clear();
	}

	public void UnloadBatch(IEnumerable<PackageInstance> packages, PackageConflictRegistry packageConflicts)
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

	public void LoadBatch(IEnumerable<PackageInstance> packages, PackageConflictRegistry packageConflicts)
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
					
					loader.LoadPackage(new PackageLoadContext(package, packageConflicts));
					loaderStopwatch.Stop();
					
					_logger.PackageLoaderCompleted(loader.GetType().Name, package.Id, loader.Stage.ToString(), loaderStopwatch.ElapsedMilliseconds);
				}
				catch (Exception ex)
				{
					package.AddIssue(PackageIssues.LoaderFailureIssue(loader.GetType().Name, ex.Message));
					throw;
				}
			}
		}
	}
}
