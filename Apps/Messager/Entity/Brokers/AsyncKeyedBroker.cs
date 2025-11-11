using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Microsoft.Extensions.Logging;

namespace Messager.Entity.Brokers;

public class AsyncKeyedBroker<TKey, TEvent> : IAsyncSender<TKey, TEvent>, IAsyncReceiver<TKey, TEvent>
	where TKey : notnull
{
	private readonly Dictionary<TKey, List<Func<TEvent, ValueTask>>> _handlers = new();
	private readonly Lock _locker = new();
	private readonly ILogger<AsyncKeyedBroker<TKey, TEvent>>? _logger;

	public AsyncKeyedBroker(ILogger<AsyncKeyedBroker<TKey, TEvent>>? logger = null)
	{
		_logger = logger;
	}
	
	public bool IsEmpty()
	{
		lock (_locker) return _handlers.Count == 0 || _handlers.All(kv => kv.Value.Count == 0);
	}

	public IAsyncDisposable Subscribe(TKey key, Func<TEvent, ValueTask> handler)
	{
		lock (_locker)
		{
			if (!_handlers.TryGetValue(key, out var list))
			{
				list = [];
				_handlers[key] = list;
				_logger?.LogDebug("Created new subscription list for key {Key} in AsyncKeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
			}
			list.Add(handler);
			_logger?.LogDebug("Subscriber added for key {Key} in AsyncKeyedBroker<{KeyType}, {EventType}>. Total subscribers for key: {Count}", key, typeof(TKey).Name, typeof(TEvent).Name, list.Count);
		}

		return new AsyncUnsubscriber(() =>
		{
			lock (_locker)
			{
				if (!_handlers.TryGetValue(key, out var list))
				{
					_logger?.LogWarning("Attempted to unsubscribe from non-existent key {Key} in AsyncKeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
					return ValueTask.CompletedTask;
				}
				
				list.Remove(handler);
				_logger?.LogDebug("Subscriber removed for key {Key} in AsyncKeyedBroker<{KeyType}, {EventType}>. Remaining subscribers for key: {Count}", key, typeof(TKey).Name, typeof(TEvent).Name, list.Count);
				
				if (list.Count == 0)
				{
					_handlers.Remove(key);
					_logger?.LogDebug("Removed empty key {Key} from AsyncKeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
				}
				
				return ValueTask.CompletedTask;
			}
		});
	}

	public void Unsubscribe(TKey key, Func<TEvent, ValueTask> handler)
	{
		lock (_locker)
		{
			if (!_handlers.TryGetValue(key, out var list))
			{
				_logger?.LogTrace("No subscription list found for key {Key} in AsyncKeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
				return;
			}
			
			var removed = list.Remove(handler);
			if (removed)
			{
				_logger?.LogDebug("Unsubscribed handler for key {Key} in AsyncKeyedBroker<{KeyType}, {EventType}>. Remaining subscribers: {Count}", key, typeof(TKey).Name, typeof(TEvent).Name, list.Count);
			}
			
			if (list.Count == 0)
			{
				_handlers.Remove(key);
				_logger?.LogDebug("Removed empty key {Key} from AsyncKeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
			}
		}
	}

	public async ValueTask SendAsync(TKey key, TEvent evt)
	{
		List<Func<TEvent, ValueTask>>? handlersCopy = null;

		lock (_locker)
		{
			if (_handlers.TryGetValue(key, out var list))
			{
				handlersCopy = list.ToList();
				_logger?.LogTrace("Sending async event {EventType} with key {Key} to {Count} subscribers", typeof(TEvent).Name, key, handlersCopy.Count);
			}
			else
			{
				_logger?.LogTrace("No subscribers found for key {Key} in AsyncKeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
			}
		}

		if (handlersCopy == null || handlersCopy.Count == 0)
			return;

		foreach (var handler in handlersCopy)
		{
			try
			{
				await handler(evt);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Error invoking async handler for event {EventType} with key {Key}", typeof(TEvent).Name, key);
			}
		}
	}
}