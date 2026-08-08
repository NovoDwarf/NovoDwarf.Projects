using Grekov.Packaging.Entities;

namespace Grekov.Packaging.Interfaces;

public interface IPackageRuntime
{
	IReadOnlyList<PackageInstance> Packages { get; }
	IReadOnlyList<PackageInstance> LoadOrder { get; }

	void LoadAll();
}
