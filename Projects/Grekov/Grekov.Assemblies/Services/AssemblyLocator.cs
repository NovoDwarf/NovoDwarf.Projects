using Grekov.Assemblies.Interfaces;
using Grekov.Packaging.Entities;

namespace Grekov.Assemblies.Services;

internal sealed class AssemblyLocator
{
	private readonly IPackagePathResolver _paths;

	public AssemblyLocator(IPackagePathResolver paths)
	{
		_paths = paths;
	}

	public IReadOnlyList<string> Locate(PackageInstance package)
	{
		var root = _paths.GetPhysicalAssembliesRoot(_paths.GetAssembliesRoot(package));
		return Directory.Exists(root)
			? Directory.EnumerateFiles(root, "*.dll", SearchOption.TopDirectoryOnly).ToArray()
			: [];
	}
}
