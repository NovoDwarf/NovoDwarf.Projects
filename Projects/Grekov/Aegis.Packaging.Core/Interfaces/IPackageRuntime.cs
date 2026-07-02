using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Core.Interfaces;

public interface IPackageRuntime
{
	public IReadOnlyList<PackageInstance> Packages { get; }
	public IReadOnlyList<PackageInstance> LoadOrder { get; }
	public IReadOnlyList<PackageConflictEvent> Conflicts { get; }

	public event Action? Changed;

	public void LoadAll();
	public void ReloadAll();
	public bool ReloadPackage(string packageId);
	public bool SetEnabled(string packageId, bool enabled);
}