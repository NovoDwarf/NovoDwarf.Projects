using Aegis.Packaging.Core.Services;
using Aegis.Packaging.Definitions.Entities;
using Aegis.Packaging.Definitions.Extensions;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Definitions.Services;

internal sealed class DefinitionIndex
{
	private readonly ILogger<DefinitionService> _logger;

	private readonly Dictionary<Type, Dictionary<string, List<DefinitionEntry>>> _defs = [];
	private readonly Dictionary<Def, DefinitionEntry> _originByDef = new();
	private readonly Dictionary<string, DefinitionEntry> _originByPath = new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<Type, Type[]> _assignableTypesCache = [];

	public DefinitionIndex(ILogger<DefinitionService> logger)
	{
		_logger = logger;
	}

	public T? Get<T>(string id) where T : Def
	{
		var resolved = ResolveByTypeAndId(typeof(T), id);
		
		return resolved as T;
	}

	public T? GetByPath<T>(string resourcePath) where T : Def
	{
		var normalizedPath = NormalizePath(resourcePath);
		
		if (normalizedPath.Length == 0)
			return null;

		return _originByPath.TryGetValue(normalizedPath, out var entry) && entry.Def is T typed
			? typed
			: null;
	}

	public IEnumerable<T> All<T>() where T : Def
	{
		foreach (var type in GetAssignableTypes(typeof(T)))
		{
			if (!_defs.TryGetValue(type, out var map))
				continue;

			foreach (var stack in map.Values.Where(stack => stack.Count != 0))
				yield return (T)stack[^1].Def;
		}
	}

	public T? FirstOrDefault<T>() where T : Def => All<T>().FirstOrDefault();

	public bool TryGetOrigin(Def? def, out string packageId, out string resourcePath)
	{
		packageId = string.Empty;
		resourcePath = string.Empty;

		if (def == null || !_originByDef.TryGetValue(def, out var entry))
			return false;

		packageId = entry.PackageId;
		resourcePath = entry.ResourcePath;
		return true;
	}

	public void Register(IEnumerable<DefinitionEntry> entries, PackageConflictRegistry packageConflicts)
	{
		foreach (var entry in entries)
		{
			using var scope = _logger.BeginScope(new Dictionary<string, object?>
			{
				["Operation"] = "defs.register",
				["PackageId"] = entry.PackageId,
				["DefType"] = entry.Def.GetType().Name,
				["DefKey"] = entry.Key,
				["ResourcePath"] = entry.ResourcePath
			});

			var type = entry.Def.GetType();
			var normalizedKey = NormalizeId(entry.Key);
			var normalizedPath = NormalizePath(entry.ResourcePath);

			if (!_defs.TryGetValue(type, out var byKey))
			{
				byKey = new Dictionary<string, List<DefinitionEntry>>(StringComparer.Ordinal);
				_defs[type] = byKey;
				InvalidateAssignableTypesCache();
			}

			if (!byKey.TryGetValue(normalizedKey, out var stack))
			{
				stack = [];
				byKey[normalizedKey] = stack;
			}

			var conflictKey = $"{type.FullName}:{normalizedKey}";
			packageConflicts.Register(conflictKey, entry.PackageId);

			stack.Add(entry);
			_originByDef[entry.Def] = entry;
			_originByPath[normalizedPath] = entry;

			_logger.DefinitionRegistered(type.Name, normalizedKey, entry.PackageId);
		}
	}

	public void Resolve(IEnumerable<PendingReference> pendingReferences)
	{
		foreach (var pending in pendingReferences)
		{
			using var scope = _logger.BeginScope(new Dictionary<string, object?>
			{
				["Operation"] = "defs.resolve",
				["OwnerPath"] = pending.OwnerPath,
				["ExpectedType"] = pending.ExpectedType.Name,
				["ReferenceId"] = pending.ReferenceId
			});
			var resolved = ResolveByTypeAndId(pending.ExpectedType, pending.ReferenceId);
			
			if (resolved == null)
			{
				_logger.DefinitionReferenceMissing(pending.OwnerPath, pending.ExpectedType.Name, NormalizeId(pending.ReferenceId));
				throw new InvalidOperationException($"Definition reference '{NormalizeId(pending.ReferenceId)}' not found for expected type '{pending.ExpectedType.Name}' in '{pending.OwnerPath}'.");
			}

			var resolvedPackageId = TryGetOrigin(resolved, out var packageId, out _) ? packageId : string.Empty;
			_logger.DefinitionReferenceResolved(
				pending.OwnerPath,
				pending.ExpectedType.Name,
				NormalizeId(pending.ReferenceId),
				resolvedPackageId);
			pending.Assign(resolved);
		}
	}

	public void Clear()
	{
		_defs.Clear();
		_originByDef.Clear();
		_originByPath.Clear();
		InvalidateAssignableTypesCache();
	}

	public void UnloadPackage(string packageId)
	{
		foreach (var type in _defs.Keys.ToList())
		{
			var byKey = _defs[type];

			foreach (var key in byKey.Keys.ToList())
			{
				var stack = byKey[key];
				stack.RemoveAll(entry => string.Equals(entry.PackageId, packageId, StringComparison.OrdinalIgnoreCase));

				if (stack.Count == 0)
					byKey.Remove(key);
			}

			if (byKey.Count == 0)
				_defs.Remove(type);
		}

		foreach (var (def, entry) in _originByDef.ToList())
		{
			if (string.Equals(entry.PackageId, packageId, StringComparison.OrdinalIgnoreCase))
				_originByDef.Remove(def);
		}

		foreach (var (path, entry) in _originByPath.ToList())
		{
			if (string.Equals(entry.PackageId, packageId, StringComparison.OrdinalIgnoreCase))
				_originByPath.Remove(path);
		}

		InvalidateAssignableTypesCache();
	}

	private Def? ResolveByTypeAndId(Type expectedType, string? id)
	{
		var normalizedId = NormalizeId(id);
		if (normalizedId.Length == 0)
			return null;

		foreach (var type in GetAssignableTypes(expectedType))
		{
			if (!_defs.TryGetValue(type, out var map))
				continue;

			if (map.TryGetValue(normalizedId, out var stack) && stack.Count > 0)
				return stack[^1].Def;
		}

		return null;
	}

	private Type[] GetAssignableTypes(Type expectedType)
	{
		if (_assignableTypesCache.TryGetValue(expectedType, out var cached))
			return cached;

		var resolved = _defs.Keys
			.Where(expectedType.IsAssignableFrom)
			.ToArray();

		_assignableTypesCache[expectedType] = resolved;
		return resolved;
	}

	private void InvalidateAssignableTypesCache()
	{
		_assignableTypesCache.Clear();
	}

	private static string NormalizeId(string? id)
	{
		return id?.Trim() ?? string.Empty;
	}

	private static string NormalizePath(string? resourcePath)
	{
		return resourcePath?.Trim() ?? string.Empty;
	}
}
