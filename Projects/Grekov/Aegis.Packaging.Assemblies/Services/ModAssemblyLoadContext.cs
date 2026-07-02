using System.Reflection;
using System.Runtime.Loader;
using Aegis.Packaging.Assemblies.Utilities;

namespace Aegis.Packaging.Assemblies;

public sealed class ModAssemblyLoadContext : AssemblyLoadContext
{
	private readonly string _assembliesDirectory;
	private readonly string _packageAssemblyName;
	private readonly string _packageId;
	private readonly IFileSystemService _fileSystem;

	private readonly AssemblyDependencyResolver _resolver;

	public ModAssemblyLoadContext(
		string packageId,
		string packageAssemblyName,
		string assemblyPath,
		string assembliesDirectory,
		IFileSystemService fileSystem)
		: base($"mod:{Path.GetFileNameWithoutExtension(assemblyPath)}", isCollectible: true)
	{
		_fileSystem = fileSystem;
		_packageId = packageId;
		_packageAssemblyName = packageAssemblyName;
		_assembliesDirectory = assembliesDirectory;
		_resolver = new AssemblyDependencyResolver(assemblyPath);
	}

	protected override Assembly? Load(AssemblyName assemblyName)
	{
		var shared = AssemblyLoadPolicy.FindLoadedAssembly(assemblyName.Name);
		if (shared != null)
			return shared;

		if (AssemblyLoadPolicy.IsReservedHostAssemblyName(assemblyName.Name, _packageAssemblyName))
			ExceptionUtils.ThrowIfResolveReservedAssembly(_packageId, assemblyName.Name ?? string.Empty);

		var path = _resolver.ResolveAssemblyToPath(assemblyName);

		if (path == null && !string.IsNullOrWhiteSpace(_assembliesDirectory))
		{
			var localPath = Path.Combine(_assembliesDirectory, $"{assemblyName.Name}.dll");
			if (_fileSystem.FileExists(localPath))
				path = localPath;
		}

		if (path == null)
			return null;

		if (AssemblyLoadPolicy.IsReservedHostAssemblyName(Path.GetFileNameWithoutExtension(path), _packageAssemblyName))
			ExceptionUtils.ThrowIfLoadingReservedAssembly(_packageId, Path.GetFileName(path), path);

		return LoadFromAssemblyPath(path);
	}
}
