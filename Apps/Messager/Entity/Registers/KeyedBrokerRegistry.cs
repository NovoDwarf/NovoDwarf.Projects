using Messager.Entity.Brokers;
using Messager.Helpers;
using Microsoft.Extensions.Logging;

namespace Messager.Entity.Registers;

internal class KeyedBrokerRegistry
{
	private readonly Dictionary<(Type, Type), object> _brokers = new();
	private readonly Lock _locker = new();
	private readonly ILoggerFactory? _loggerFactory;

	public KeyedBrokerRegistry(ILoggerFactory? loggerFactory = null)
	{
		_loggerFactory = loggerFactory;
	}

	public KeyedBroker<TKey, TEvent> GetOrCreate<TKey, TEvent>() where TKey : notnull
	{
		lock (_locker)
		{
			var key = (typeof(TKey), typeof(TEvent));
			
			return RegistryHelper.GetOrCreate(_brokers, key, () =>
			{
				var logger = _loggerFactory?.CreateLogger<KeyedBroker<TKey, TEvent>>();
				return new KeyedBroker<TKey, TEvent>(logger);
			});
		}
	}
}