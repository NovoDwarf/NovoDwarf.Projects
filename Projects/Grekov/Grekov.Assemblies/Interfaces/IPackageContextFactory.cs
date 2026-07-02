using Grekov.Core.Interfaces;
using Grekov.Packaging.Entities;

namespace Grekov.Assemblies.Interfaces;

public interface IPackageContextFactory
{
	IPackageContext Create(PackageInstance package);
}
