using Aegis.Packaging.Assemblies.Extensions;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Assemblies;

public sealed class EntrypointActivator
{
	private readonly ILogger<AssemblyService> _logger;

	public EntrypointActivator(ILogger<AssemblyService> logger)
	{
		_logger = logger;
	}

	public List<IEntrypoint> CreateEntrypoints(
		IReadOnlyList<Type> entrypointTypes,
		PackageInstance package,
		IPackageApi packageApi,
		PackageConflictRegistry packageConflicts,
		string primaryAssemblyPath,
		bool isReload)
	{
		var instances = new List<IEntrypoint>(entrypointTypes.Count);

		foreach (var entrypointType in entrypointTypes)
		{
			packageConflicts.Register($"mod-entry:{entrypointType.FullName}", package.Id);

			if (Activator.CreateInstance(entrypointType) is not IEntrypoint entrypoint)
			{
				_logger.EntrypointHasNoPublicConstructor(entrypointType.FullName, primaryAssemblyPath);
				continue;
			}

			if (entrypoint is IPackageLifecycle lifecycle)
				lifecycle.OnPackageLoading();

			if (isReload && entrypoint is IPackageReloadLifecycle reloadLifecycle)
				reloadLifecycle.OnPackageReloading();

			entrypoint.Initialize(packageApi);

			if (entrypoint is IPackageLifecycle loadedLifecycle)
				loadedLifecycle.OnPackageLoaded();

			if (isReload && entrypoint is IPackageReloadLifecycle loadedReloadLifecycle)
				loadedReloadLifecycle.OnPackageReloaded();

			instances.Add(entrypoint);
		}

		return instances;
	}
}
