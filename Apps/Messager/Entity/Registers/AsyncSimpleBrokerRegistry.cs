using Messager.Entity.Brokers;
using Messager.Entity.Helpers;

namespace Messager.Entity.Registers;

internal class AsyncSimpleBrokerRegistry
{
	private readonly Dictionary<Type, object> _brokers = new();
	private readonly object _locker = new();

	public AsyncSimpleBroker<TEvent> GetOrCreate<TEvent>()
	{
		lock (_locker)
		{
			return RegistryHelper.GetOrCreate(_brokers, typeof(TEvent), () => new AsyncSimpleBroker<TEvent>());
		}
	}

	public List<(string, int)> GetStats()
	{
		lock (_locker)
		{
			return RegistryHelper.CollectStats(_brokers, t => t.Name);
		}
	}
}