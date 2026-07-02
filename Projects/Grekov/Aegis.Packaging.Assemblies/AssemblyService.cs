using System.Reflection;
using Aegis.Packaging.Assemblies.Entities;
using Aegis.Packaging.Assemblies.Extensions;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Interfaces;
using Aegis.Packaging.Core.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Assemblies;

public sealed class AssemblyService : IPackageContentLoader
{
	private readonly Dictionary<string, List<LoadedPackageHandle>> _loadedByPackage = new(StringComparer.OrdinalIgnoreCase);
	private readonly HashSet<string> _reloadingPackages = new(StringComparer.OrdinalIgnoreCase);

	private readonly IFileSystemService _fileSystem;
	private readonly ILogger<AssemblyService> _logger;
	private readonly IApiFactory _apiFactory;
	private readonly ISceneRegistry _gameScenes;
	private readonly IPackageServiceRegistry _packageServices;

	private readonly PackageAssemblyLocator _assemblyLocator;
	private readonly EntrypointResolver _entrypointResolver;
	private readonly EntrypointActivator _entrypointActivator;

	public AssemblyService(
		ILogger<AssemblyService> logger,
		IApiFactory apiFactory,
		ISceneRegistry gameScenes,
		IFileSystemService fileSystem,
		IPackageServiceRegistry packageServices)
	{
		_fileSystem = fileSystem;
		_logger = logger;
		_apiFactory = apiFactory;
		_gameScenes = gameScenes;
		_packageServices = packageServices;

		_assemblyLocator = new PackageAssemblyLocator(fileSystem);
		_entrypointResolver = new EntrypointResolver(logger);
		_entrypointActivator = new EntrypointActivator(logger);
	}

	public PackageContentStage Stage => PackageContentStage.Assemblies;

	public void Clear()
	{
		foreach (var packageId in _loadedByPackage.Keys.ToArray())
			_packageServices.UnregisterAll(packageId);

		foreach (var packageId in _loadedByPackage.Keys.ToArray())
			_gameScenes.UnloadPackage(packageId);

		foreach (var loaded in _loadedByPackage.Values.SelectMany(static x => x))
			loaded.Dispose();

		_loadedByPackage.Clear();
		_reloadingPackages.Clear();
	}

	public void LoadPackage(PackageInstance package, PackageConflictRegistry packageConflicts)
	{
		var assemblySet = _assemblyLocator.Locate(package);

		if (assemblySet == null || assemblySet.AssemblyPaths.Count == 0)
		{
			_reloadingPackages.Remove(package.Id);
			return;
		}

		var isReload = _reloadingPackages.Contains(package.Id);
		var packageApi = _apiFactory.Create(package.Id);

		var loaded = TryLoadPackageAssemblies(package, assemblySet, packageApi, packageConflicts, isReload);
		if (loaded != null)
			_loadedByPackage[package.Id] = [loaded];

		_reloadingPackages.Remove(package.Id);
	}

	public void UnloadPackage(string packageId)
	{
		_packageServices.UnregisterAll(packageId);

		if (!_loadedByPackage.TryGetValue(packageId, out var loadedAssemblies))
		{
			_gameScenes.UnloadPackage(packageId);
			return;
		}

		foreach (var loaded in loadedAssemblies)
			loaded.Dispose();

		_loadedByPackage.Remove(packageId);
		_reloadingPackages.Add(packageId);
		_gameScenes.UnloadPackage(packageId);
	}

	private LoadedPackageHandle? TryLoadPackageAssemblies(
		PackageInstance package,
		PackageAssemblySet assemblySet,
		IPackageApi packageApi,
		PackageConflictRegistry packageConflicts,
		bool isReload)
	{
		var primaryAssemblyPath = assemblySet.PrimaryAssemblyPath;
		var assembliesDirectory = Path.GetDirectoryName(primaryAssemblyPath) ?? string.Empty;
		var primaryAssemblyName = Path.GetFileNameWithoutExtension(primaryAssemblyPath) ?? package.Id.Trim();

		var loadContext = new ModAssemblyLoadContext(
			package.Id,
			primaryAssemblyName,
			primaryAssemblyPath,
			assembliesDirectory,
			_fileSystem);

		try
		{
			var entrypointTypes = _entrypointResolver.ResolveEntrypointTypes(
				package,
				loadContext,
				assemblySet.AssemblyPaths,
				primaryAssemblyPath);

			if (entrypointTypes.Length == 0)
			{
				_logger.LoadedPackageWithoutEntrypoint(primaryAssemblyPath, package.Id);
				return new LoadedPackageHandle(loadContext, []);
			}

			var entrypoints = _entrypointActivator.CreateEntrypoints(
				entrypointTypes,
				package,
				packageApi,
				packageConflicts,
				primaryAssemblyPath,
				isReload);

			if (entrypoints.Count == 0)
			{
				loadContext.Unload();
				return null;
			}

			_logger.LoadedEntrypoint(entrypoints.Count, package.Id, primaryAssemblyPath);
			return new LoadedPackageHandle(loadContext, entrypoints);
		}
		catch (ReflectionTypeLoadException ex)
		{
			foreach (var loaderException in ex.LoaderExceptions)
				_logger.FailedToLoadAssemblyDependencies(primaryAssemblyPath, loaderException);

			loadContext.Unload();
			throw;
		}
		catch (Exception ex)
		{
			_logger.FailedToLoadAssembly(primaryAssemblyPath, ex);
			loadContext.Unload();
			throw;
		}
	}
}
