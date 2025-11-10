using Messager.Entity.Brokers;
using Messager.Entity.Helpers;

namespace Messager.Entity.Registers;

internal class SimpleBrokerRegistry
{
	private readonly Dictionary<Type, object> _brokers = new();
	private readonly object _locker = new();

	public SimpleBroker<TEvent> GetOrCreate<TEvent>()
	{
		lock (_locker)
		{
			return RegistryHelper.GetOrCreate(_brokers, typeof(TEvent), () => new SimpleBroker<TEvent>());
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