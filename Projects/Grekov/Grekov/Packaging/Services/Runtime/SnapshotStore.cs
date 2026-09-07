using Grekov.Packaging.Entities;

namespace Grekov.Packaging.Services.Runtime;

internal sealed class SnapshotStore
{
	private PackageSnapshot? _snapshot;

	public PackageSnapshot? Current => _snapshot;

	public void Capture(IReadOnlyList<PackageInstance> packages, IReadOnlyList<PackageInstance> loadOrder)
	{
		_snapshot = new PackageSnapshot(packages.ToArray(), loadOrder.ToArray());
	}
}
