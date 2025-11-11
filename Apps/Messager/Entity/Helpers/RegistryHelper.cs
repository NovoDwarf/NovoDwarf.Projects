using Messager.Interfaces.Services;

namespace Messager.Entity.Helpers;

internal static class RegistryHelper
{
	public static TBroker GetOrCreate<TKey, TBroker>(Dictionary<TKey, object> dict, TKey key, Func<TBroker> factory)
		where TBroker : class, IBrokerInfo where TKey : notnull
	{
		if (dict.TryGetValue(key, out var existing) && existing is TBroker broker)
			return broker;

		var newBroker = factory();
		dict[key] = newBroker;
		return newBroker;
	}
}