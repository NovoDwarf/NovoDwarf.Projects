using Grekov.Packaging.Constants;
using Grekov.Packaging.Entities;
using Grekov.Packaging.Interfaces;

namespace Grekov.Assemblies.Services;

internal sealed class AssemblyService : IPackageContentLoader
{
	private readonly AssemblyLocator _locator;

	public AssemblyService(AssemblyLocator locator)
	{
		_locator = locator;
	}

	public int Stage => PackageStages.Runtime;

	public void LoadPackage(PackageLoadContext context)
	{
		_ = _locator.Locate(context.Package);
	}

	public void UnloadPackage(string packageId)
	{
		
	}
}
