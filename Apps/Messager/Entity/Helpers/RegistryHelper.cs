using Messager.Interfaces.Services;

namespace Messager.Entity.Helpers;

internal static class RegistryHelper
{
	public static TBroker GetOrCreate<TKey, TBroker>(Dictionary<TKey, object> dict, TKey key, Func<TBroker> factory)
		where TBroker : class, IBrokerInfo where TKey : notnull
	{
		if (dict.TryGetValue(key, out var existing) && existing is TBroker broker && !broker.IsEmpty())
			return broker;

		var newBroker = factory();
		dict[key] = newBroker;
		return newBroker;
	}

	public static List<(string, int)> CollectStats<TKey>(Dictionary<TKey, object> dict, Func<TKey, string> nameSelector)
		where TKey : notnull
	{
		var result = new List<(string, int)>();
		foreach (var kv in dict)
		{
			if (kv.Value is IBrokerInfo dbg)
				result.Add((nameSelector(kv.Key), dbg.SubscriberCount));
		}
		return result;
	}
}