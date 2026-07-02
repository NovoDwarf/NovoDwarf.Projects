using Grekov.Assemblies.Interfaces;
using Grekov.Packaging.Entities;
using NovoDwarf.FS.Interfaces;

namespace Grekov.Assemblies.Services;

public sealed class DefaultPackageAssemblyPathResolver : IPackageAssemblyPathResolver
{
	private const string AssembliesFolderName = "Assemblies";

	private readonly IPathService _pathService;

	public DefaultPackageAssemblyPathResolver(IPathService pathService)
	{
		_pathService = pathService;
	}

	public string GetPackageAssembliesRoot(PackageInstance package)
	{
		return _pathService.Combine(package.RootPath, AssembliesFolderName);
	}

	public string GetPhysicalAssembliesRoot(string packageAssembliesRoot)
	{
		return packageAssembliesRoot;
	}
}
