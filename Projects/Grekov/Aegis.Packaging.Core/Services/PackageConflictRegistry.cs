using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Core.Services;

public sealed class PackageConflictRegistry
{
	private readonly ILogger<PackageConflictRegistry> _logger;
	private readonly List<PackageConflictEvent> _events = [];
	private readonly Dictionary<string, List<string>> _owners = [];
	private readonly Dictionary<string, HashSet<string>> _keysByPackage = new(StringComparer.OrdinalIgnoreCase);

	public PackageConflictRegistry(ILogger<PackageConflictRegistry> logger)
	{
		_logger = logger;
	}

	public IReadOnlyList<PackageConflictEvent> Events => _events;

	public void Clear()
	{
		_owners.Clear();
		_keysByPackage.Clear();
		_events.Clear();
	}

	public void Register(string key, string packageId)
	{
		if (!_owners.TryGetValue(key, out var stack))
		{
			stack = [];
			_owners[key] = stack;
		}

		if (stack.Count > 0)
			_logger.PackageConflictOverride(key, packageId, stack[^1]);

		stack.Add(packageId);

		if (!_keysByPackage.TryGetValue(packageId, out var keys))
		{
			keys = new HashSet<string>(StringComparer.Ordinal);
			_keysByPackage[packageId] = keys;
		}

		keys.Add(key);
		RebuildEvents();
	}

	public void UnregisterPackage(string packageId)
	{
		if (!_keysByPackage.TryGetValue(packageId, out var keys))
			return;

		foreach (var key in keys)
		{
			if (!_owners.TryGetValue(key, out var stack))
				continue;

			stack.RemoveAll(owner => string.Equals(owner, packageId, StringComparison.OrdinalIgnoreCase));
		
			if (stack.Count == 0)
				_owners.Remove(key);
		}

		_keysByPackage.Remove(packageId);
		RebuildEvents();
	}

	private void RebuildEvents()
	{
		_events.Clear();

		foreach (var (key, stack) in _owners)
		{
			if (stack.Count < 2)
				continue;

			for (var index = 1; index < stack.Count; index++)
				_events.Add(new PackageConflictEvent(key, stack[index], stack[index - 1]));
		}
	}
}

