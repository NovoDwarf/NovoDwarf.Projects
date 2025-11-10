using Messager.Core;
using Messager.Entity.Resources;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Messager.Interfaces.Services;

namespace Messager.Entity.Brokers;

public class SimpleBroker<TEvent> : ISender<TEvent>, IReceiver<TEvent>, IBrokerInfo
{
	private readonly List<WeakAction<TEvent>> _handlers = [];
	private readonly object _locker = new();

	public int SubscriberCount
	{
		get { lock (_locker) return _handlers.Count; }
	}
	
	public string DebugInfo()
	{
		lock (_locker)
		{
			return $"SimpleBroker<{typeof(TEvent).Name}>: {SubscriberCount} subscriber(s)";
		}
	}
	
	public void Send(TEvent evt)
	{
		lock (_locker)
		{
			_handlers.RemoveAll(s => !s.IsAlive);

			foreach (var sub in _handlers.ToList())
				sub.TryInvoke(evt);
		}
	}

	public IDisposable Subscribe(Action<TEvent> handler)
	{
		lock (_locker)
		{
			_handlers.Add(new WeakAction<TEvent>(handler));
		}

		return new Unsubscriber(() =>
		{
			lock (_locker)
			{
				_handlers.Remove(new WeakAction<TEvent>(handler));
			}
		});
	}

	public void Unsubscribe(Action<TEvent> handler)
	{
		lock (_locker)
			_handlers.RemoveAll(s => s.Matches(handler));
	}

	public bool IsEmpty()
	{
		lock (_locker)
		{
			_handlers.RemoveAll(s => !s.IsAlive);
			return _handlers.Count == 0;
		}
	}
}