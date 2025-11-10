using Messager.Core;
using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Messager.Interfaces.Services;

namespace Messager.Entity.Brokers;

public class KeyedBroker<TKey, TEvent> : ISender<TKey, TEvent>, IReceiver<TKey, TEvent>, IBrokerInfo where TKey : notnull
{
	private readonly Dictionary<TKey, List<WeakAction<TEvent>>> _handlers = new();
	private readonly object _locker = new();

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
				return;
			
			list.RemoveAll(s => !s.IsAlive);
			
			foreach (var sub in list.ToList())
				sub.TryInvoke(evt);

			if (list.Count == 0)
				_handlers.Remove(key);
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
			}
			list.Add(new WeakAction<TEvent>(handler));
		}

		return new Unsubscriber(() =>
		{
			lock (_locker)
			{
				if (!_handlers.TryGetValue(key, out var list)) 
					return;
				
				list.Remove(new WeakAction<TEvent>(handler));
				
				if (list.Count == 0)
					_handlers.Remove(key);
			}
		});
	}

	public void Unsubscribe(TKey key, Action<TEvent> handler)
	{
		lock (_locker)
		{
			if (!_handlers.TryGetValue(key, out var list)) 
				return;
			
			list.RemoveAll(s => s.Matches(handler) || !s.IsAlive);
			
			if (list.Count == 0)
				_handlers.Remove(key);
		}
	}
}