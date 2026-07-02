using Grekov.Core.Interfaces;

namespace Grekov.Assemblies.Entities;

public sealed class PackageContext : IPackageContext
{
	public PackageContext(string packageId, string rootPath, IServiceProvider services)
	{
		PackageId = packageId;
		RootPath = rootPath;
		Services = services;
	}

	public string PackageId { get; }
	public string RootPath { get; }
	public IServiceProvider Services { get; }
}
