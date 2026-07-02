using System.Runtime.Loader;

namespace Aegis.Packaging.Assemblies.Entities;

public sealed class LoadedPackageHandle : IDisposable
{
	private readonly IReadOnlyList<IEntrypoint> _entrypoints;
	private readonly AssemblyLoadContext _loadContext;

	public LoadedPackageHandle(AssemblyLoadContext loadContext, IReadOnlyList<IEntrypoint> entrypoints)
	{
		_loadContext = loadContext;
		_entrypoints = entrypoints;
	}

	public void Dispose()
	{
		foreach (var entrypoint in _entrypoints)
		{
			if (entrypoint is IPackageLifecycle lifecycle)
				lifecycle.OnPackageUnloading();

			if (entrypoint is IDisposable disposable)
				disposable.Dispose();

			if (entrypoint is IPackageLifecycle unloadedLifecycle)
				unloadedLifecycle.OnPackageUnloaded();
		}

		if (_loadContext.IsCollectible)
			_loadContext.Unload();
	}
}
