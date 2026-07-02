using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Enums;
using Aegis.Packaging.Core.Services;

namespace Aegis.Packaging.Core.Interfaces;

public interface IPackageContentLoader
{
	public PackageContentStage Stage => PackageContentStage.Definitions;
	public int Order => 0;
	public void Clear();
	public void LoadPackage(PackageInstance package, PackageConflictRegistry packageConflicts);
	public void UnloadPackage(string packageId) {}
	public void BeginTransaction() {}
	public void CommitTransaction() {}
	public void RollbackTransaction() {}
}
