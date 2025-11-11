using Messager.Entity.Brokers;
using Messager.Entity.Helpers;
using Microsoft.Extensions.Logging;

namespace Messager.Entity.Registers;

internal class AsyncKeyedBrokerRegistry
{
	private readonly Dictionary<(Type, Type), object> _brokers = new();
	private readonly Lock _locker = new();
	private readonly ILoggerFactory? _loggerFactory;

	public AsyncKeyedBrokerRegistry(ILoggerFactory? loggerFactory = null)
	{
		_loggerFactory = loggerFactory;
	}

	public AsyncKeyedBroker<TKey, TEvent> GetOrCreate<TKey, TEvent>() where TKey : notnull
	{
		lock (_locker)
		{
			var key = (typeof(TKey), typeof(TEvent));
			return RegistryHelper.GetOrCreate(_brokers, key, () =>
			{
				var logger = _loggerFactory?.CreateLogger<AsyncKeyedBroker<TKey, TEvent>>();
				
				return new AsyncKeyedBroker<TKey, TEvent>(logger);
			});
		}
	}
}