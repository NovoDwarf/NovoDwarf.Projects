using Messager.Core;
using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Messager.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Messager.Entity.Brokers;

public class KeyedBroker<TKey, TEvent> : ISender<TKey, TEvent>, IReceiver<TKey, TEvent>, IBrokerInfo where TKey : notnull
{
	private readonly Dictionary<TKey, List<WeakAction<TEvent>>> _handlers = new();
	private readonly Lock _locker = new();
	private readonly ILogger<KeyedBroker<TKey, TEvent>>? _logger;

	public KeyedBroker(ILogger<KeyedBroker<TKey, TEvent>>? logger = null)
	{
		_logger = logger;
	}

	public int SubscriberCount
	{
		get { lock (_locker) return _handlers.Values.Sum(l => l.Count); }
	}
	
	public string DebugInfo()
	{
		lock (_locker)
		{
			return $"KeyedBroker<{typeof(TKey).Name}, {typeof(TEvent).Name}>: {SubscriberCount} subscriber(s) across {_handlers.Count} key(s)";
		}
	}
	
	public bool IsEmpty()
	{
		lock (_locker)
		{
			foreach (var key in _handlers.Keys.ToList())
			{
				_handlers[key].RemoveAll(s => !s.IsAlive);
				
				if (_handlers[key].Count == 0)
					_handlers.Remove(key);
			}
			return _handlers.Count == 0;
		}
	}
	
	public void Send(TKey key, TEvent evt)
	{
		lock (_locker)
		{
			if (!_handlers.TryGetValue(key, out var list))
			{
				_logger?.LogTrace("No subscribers found for key {Key} in KeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
				return;
			}
			
			var removedCount = list.RemoveAll(s => !s.IsAlive);
			if (removedCount > 0)
			{
				_logger?.LogDebug("Removed {Count} dead subscribers for key {Key} in KeyedBroker<{KeyType}, {EventType}>", removedCount, key, typeof(TKey).Name, typeof(TEvent).Name);
			}
			
			var activeHandlers = list.ToList();
			_logger?.LogTrace("Sending event {EventType} with key {Key} to {Count} subscribers", typeof(TEvent).Name, key, activeHandlers.Count);
			
			foreach (var sub in activeHandlers)
			{
				try
				{
					sub.TryInvoke(evt);
				}
				catch (Exception ex)
				{
					_logger?.LogError(ex, "Error invoking handler for event {EventType} with key {Key}", typeof(TEvent).Name, key);
				}
			}

			if (list.Count == 0)
			{
				_handlers.Remove(key);
				_logger?.LogDebug("Removed empty key {Key} from KeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
			}
		}
	}

	public IDisposable Subscribe(TKey key, Action<TEvent> handler)
	{
		lock (_locker)
		{
			if (!_handlers.TryGetValue(key, out var list))
			{
				list = [];
				_handlers[key] = list;
				_logger?.LogDebug("Created new subscription list for key {Key} in KeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
			}
			list.Add(new WeakAction<TEvent>(handler));
			_logger?.LogDebug("Subscriber added for key {Key} in KeyedBroker<{KeyType}, {EventType}>. Total subscribers for key: {Count}", key, typeof(TKey).Name, typeof(TEvent).Name, list.Count);
		}

		return new Unsubscriber(() =>
		{
			lock (_locker)
			{
				if (!_handlers.TryGetValue(key, out var list)) 
					return;
				
				list.Remove(new WeakAction<TEvent>(handler));
				_logger?.LogDebug("Subscriber removed for key {Key} in KeyedBroker<{KeyType}, {EventType}>. Remaining subscribers for key: {Count}", key, typeof(TKey).Name, typeof(TEvent).Name, list.Count);
				
				if (list.Count == 0)
				{
					_handlers.Remove(key);
					_logger?.LogDebug("Removed empty key {Key} from KeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
				}
			}
		});
	}

	public void Unsubscribe(TKey key, Action<TEvent> handler)
	{
		lock (_locker)
		{
			if (!_handlers.TryGetValue(key, out var list))
			{
				_logger?.LogTrace("No subscription list found for key {Key} in KeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
				return;
			}
			
			var removedCount = list.RemoveAll(s => s.Matches(handler) || !s.IsAlive);
			if (removedCount > 0)
			{
				_logger?.LogDebug("Unsubscribed {Count} handler(s) for key {Key} in KeyedBroker<{KeyType}, {EventType}>. Remaining subscribers: {Count}", removedCount, key, typeof(TKey).Name, typeof(TEvent).Name, list.Count);
			}
			
			if (list.Count == 0)
			{
				_handlers.Remove(key);
				_logger?.LogDebug("Removed empty key {Key} from KeyedBroker<{KeyType}, {EventType}>", key, typeof(TKey).Name, typeof(TEvent).Name);
			}
		}
	}
}