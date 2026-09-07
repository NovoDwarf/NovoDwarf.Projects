using Grekov.Packaging.Interfaces;
using Grekov.Packaging.Services;

namespace Grekov.Packaging.Services.Conflicts;

internal sealed class PackageConflictRegistry : IPackageConflictSink
{
	private readonly Dictionary<string, string> _ownersByKey = new(StringComparer.OrdinalIgnoreCase);

	public void Register(string key, string packageId)
	{
		if (!_ownersByKey.TryGetValue(key, out var ownerPackageId))
		{
			_ownersByKey[key] = packageId;
			return;
		}

		if (!string.Equals(ownerPackageId, packageId, StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException(PackageIssues.ConflictIssue(key, packageId, ownerPackageId).Message);
	}

	public void UnregisterPackage(string packageId)
	{
		foreach (var key in _ownersByKey.Where(pair => string.Equals(pair.Value, packageId, StringComparison.OrdinalIgnoreCase)).Select(static pair => pair.Key).ToArray())
			_ownersByKey.Remove(key);
	}

	public void Clear()
	{
		_ownersByKey.Clear();
	}
}
