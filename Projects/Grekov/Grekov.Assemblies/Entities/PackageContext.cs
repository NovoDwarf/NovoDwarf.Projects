using Grekov.Core.Interfaces;

namespace Grekov.Assemblies.Entities;

public sealed class PackageContext : IPackageContext
{
	public PackageContext(string packageId, string root, IServiceProvider services)
	{
		PackageId = packageId;
		Root = root;
		Services = services;
	}

	public string PackageId { get; }
	public string Root { get; }
	
	public IServiceProvider Services { get; }
}


