using Grekov.Packaging.Entities;

namespace Grekov.Packaging.Interfaces;

public interface IPackageContentLoader
{
	public int Stage { get; }
	public int Order => 0;

	void BeginTransaction()
	{
	}

	void CommitTransaction()
	{
	}

	void RollbackTransaction()
	{
	}

	void Clear()
	{
	}

	void LoadPackage(PackageLoadContext context);
	void UnloadPackage(string packageId);
}
