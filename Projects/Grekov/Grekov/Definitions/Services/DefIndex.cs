using Grekov.Core;

namespace Grekov.Definitions.Services;

internal sealed class DefIndex
{
	private readonly Dictionary<Type, Dictionary<string, Def>> _defsByTypeAndId = [];
	private readonly Dictionary<Def, (string PackageId, string ResourcePath)> _origins = [];

	public void Add(Def def)
	{
		var type = def.GetType();
		while (type != null && typeof(Def).IsAssignableFrom(type))
		{
			var bucket = GetBucket(type);
			bucket[def.Id.ToString()] = def;
			type = type.BaseType;
		}

		_origins[def] = (def.PackageId, def.ResourcePath);
	}

	public void RemovePackage(string packageId)
	{
		foreach (var bucket in _defsByTypeAndId.Values)
		foreach (var pair in bucket.Where(pair => string.Equals(pair.Value.PackageId, packageId, StringComparison.OrdinalIgnoreCase)).ToArray())
			bucket.Remove(pair.Key);

		foreach (var pair in _origins.Where(pair => string.Equals(pair.Value.PackageId, packageId, StringComparison.OrdinalIgnoreCase)).ToArray())
			_origins.Remove(pair.Key);
	}

	public void Clear()
	{
		_defsByTypeAndId.Clear();
		_origins.Clear();
	}

	public T? Get<T>(string id) where T : Def
	{
		return Get(typeof(T), id) as T;
	}

	public Def? Get(Type type, string id)
	{
		return _defsByTypeAndId.TryGetValue(type, out var bucket) && bucket.TryGetValue(id, out var def)
			? def
			: null;
	}

	public T? GetByPath<T>(string? resourcePath) where T : Def
	{
		if (string.IsNullOrWhiteSpace(resourcePath))
			return null;

		return All<T>().FirstOrDefault(def => string.Equals(def.ResourcePath, resourcePath, StringComparison.OrdinalIgnoreCase));
	}

	public IEnumerable<T> All<T>() where T : Def
	{
		return _defsByTypeAndId.TryGetValue(typeof(T), out var bucket)
			? bucket.Values.Cast<T>()
			: [];
	}

	public bool TryGetOrigin(Def def, out string packageId, out string resourcePath)
	{
		if (_origins.TryGetValue(def, out var origin))
		{
			packageId = origin.PackageId;
			resourcePath = origin.ResourcePath;
			return true;
		}

		packageId = string.Empty;
		resourcePath = string.Empty;
		return false;
	}

	private Dictionary<string, Def> GetBucket(Type type)
	{
		if (!_defsByTypeAndId.TryGetValue(type, out var bucket))
		{
			bucket = new Dictionary<string, Def>(StringComparer.OrdinalIgnoreCase);
			_defsByTypeAndId[type] = bucket;
		}

		return bucket;
	}
}
