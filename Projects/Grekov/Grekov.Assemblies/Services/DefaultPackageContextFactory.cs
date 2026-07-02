using Grekov.Assemblies.Entities;
using Grekov.Assemblies.Interfaces;
using Grekov.Core.Interfaces;
using Grekov.Packaging.Entities;

namespace Grekov.Assemblies.Services;

public sealed class DefaultPackageContextFactory : IPackageContextFactory
{
	private readonly IServiceProvider _services;

	public DefaultPackageContextFactory(IServiceProvider services)
	{
		_services = services;
	}

	public IPackageContext Create(PackageInstance package)
	{
		return new PackageContext(package.Id, package.RootPath, _services);
	}
}
