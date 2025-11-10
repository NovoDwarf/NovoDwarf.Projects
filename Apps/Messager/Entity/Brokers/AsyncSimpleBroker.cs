using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Messager.Interfaces.Services;

namespace Messager.Entity.Brokers;

public class AsyncSimpleBroker<TEvent> : IAsyncSender<TEvent>, IAsyncReceiver<TEvent>, IBrokerInfo
{
	private readonly List<Func<TEvent, ValueTask>> _handlers = [];
	private readonly object _locker = new();

	public int SubscriberCount
	{
		get { lock (_locker) return _handlers.Count; }
	}
	
	public bool IsEmpty()
	{
		lock (_locker) return _handlers.Count == 0;
	}

	public IAsyncDisposable Subscribe(Func<TEvent, ValueTask> handler)
	{
		lock (_locker)
		{
			_handlers.Add(handler);
		}

		return new AsyncUnsubscriber(() =>
		{
			lock (_locker)
			{
				_handlers.Remove(handler);

				return ValueTask.CompletedTask;
			}
		});
	}

	public void Unsubscribe(Func<TEvent, ValueTask> handler)
	{
		lock (_locker)
		{
			_handlers.Remove(handler);
		}
	}

	public async ValueTask SendAsync(TEvent evt)
	{
		List<Func<TEvent, ValueTask>> handlersCopy;
		
		lock (_locker)
		{
			handlersCopy = _handlers.ToList();
		}

		foreach (var handler in handlersCopy)
		{
			await handler(evt);
		}
	}
}