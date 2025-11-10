using Messager.Entity.Brokers;
using Messager.Entity.Helpers;

namespace Messager.Entity.Registers;

internal class AsyncKeyedBrokerRegistry
{
	private readonly Dictionary<(Type, Type), object> _brokers = new();
	private readonly object _locker = new();

	public AsyncKeyedBroker<TKey, TEvent> GetOrCreate<TKey, TEvent>() where TKey : notnull
	{
		lock (_locker)
		{
			var key = (typeof(TKey), typeof(TEvent));
			return RegistryHelper.GetOrCreate(_brokers, key, () => new AsyncKeyedBroker<TKey, TEvent>());
		}
	}

	public List<(string, int)> GetStats()
	{
		lock (_locker)
		{
			return RegistryHelper.CollectStats(_brokers, k => $"{k.Item2.Name} <{k.Item1.Name}>");
		}
	}
}