using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Core.Interfaces;

public interface IPackageCatalog
{
	public IReadOnlyList<PackageInstance> DiscoverPackages();
}