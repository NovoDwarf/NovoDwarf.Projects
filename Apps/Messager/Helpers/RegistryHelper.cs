namespace Messager.Helpers;

internal static class RegistryHelper
{
	public static TBroker GetOrCreate<TKey, TBroker>(Dictionary<TKey, object> dict, TKey key, Func<TBroker> factory)
		where TBroker : class where TKey : notnull
	{
		if (dict.TryGetValue(key, out var existing) && existing is TBroker broker)
			return broker;

		var newBroker = factory();
		dict[key] = newBroker;
		return newBroker;
	}
}