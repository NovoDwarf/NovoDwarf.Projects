using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Messager.Interfaces.Services;

namespace Messager.Entity.Brokers;

public class AsyncKeyedBroker<TKey, TEvent> : IAsyncSender<TKey, TEvent>, IAsyncReceiver<TKey, TEvent>, IBrokerInfo
	where TKey : notnull
{
	private readonly Dictionary<TKey, List<Func<TEvent, ValueTask>>> _handlers = new();
	private readonly Lock _locker = new();

	public int SubscriberCount
	{
		get { lock (_locker) return _handlers.Count; }
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
			}
			list.Add(handler);
		}

		return new AsyncUnsubscriber(() =>
		{
			lock (_locker)
			{
				if (!_handlers.TryGetValue(key, out var list)) 
					return ValueTask.FromException(new KeyNotFoundException());
				
				list.Remove(handler);
				
				if (list.Count == 0)
					_handlers.Remove(key);
				
				return ValueTask.CompletedTask;
			}
		});
	}

	public void Unsubscribe(TKey key, Func<TEvent, ValueTask> handler)
	{
		lock (_locker)
		{
			if (!_handlers.TryGetValue(key, out var list)) 
				return;
			
			list.Remove(handler);
			if (list.Count == 0)
				_handlers.Remove(key);
		}
	}

	public async ValueTask SendAsync(TKey key, TEvent evt)
	{
		List<Func<TEvent, ValueTask>>? handlersCopy = null;

		lock (_locker)
		{
			if (_handlers.TryGetValue(key, out var list))
				handlersCopy = list.ToList();
		}

		if (handlersCopy == null || handlersCopy.Count == 0)
			return;

		foreach (var handler in handlersCopy)
		{
			await handler(evt);
		}
	}
}