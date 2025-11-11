using Messager.Core;
using Messager.Entity.Resources;
using Messager.Extensions;
using Messager.Interfaces.Core;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Microsoft.Extensions.Logging;

namespace Messager.Entity.Brokers;

public class SimpleBroker<TEvent> : IBroker<TEvent>, ISender<TEvent>, IReceiver<TEvent>
{
	private readonly List<WeakAction<TEvent>> _handlers = [];
	private readonly Lock _locker = new();
	private readonly ILogger<SimpleBroker<TEvent>>? _logger;

	public SimpleBroker(ILogger<SimpleBroker<TEvent>>? logger = null)
	{
		_logger = logger;
	}

	public Guid Id { get; set; } = Guid.NewGuid();
	
	public string BrokerType => typeof(SimpleBroker<>).Name;
	public string EventType => typeof(TEvent).Name;
	
	public void Send(TEvent evt)
	{
		TryRemove();
		
		lock (_locker)
		{
			_logger?.LogSendingEvent(BrokerType, EventType, Id, _handlers.Count);
			
			foreach (var sub in _handlers)
			{
				TryInvoke(sub, evt);
			}
		}
	}

	public IDisposable Subscribe(Action<TEvent> handler)
	{
		lock (_locker)
		{
			_handlers.Add(new WeakAction<TEvent>(handler));
			_logger?.LogSubscriberAdded(BrokerType, EventType, Id);
		}

		return new Unsubscriber(() =>
		{
			lock (_locker)
			{
				_handlers.RemoveAll(s => s.Matches(handler));
				_logger?.LogSubscriberRemoved(BrokerType, EventType, Id);
			}
		});
	}

	private void TryRemove()
	{
		var removedCount = _handlers.RemoveAll(s => !s.IsAlive);

		if (removedCount <= 0) 
			return;
		
		lock (_locker)
		{
			_logger?.LogRemovedDeadSubscribers(BrokerType, EventType, Id, removedCount);
		}
	}

	private void TryInvoke(WeakAction<TEvent> sub, TEvent evt)
	{
		try
		{
			sub.TryInvoke(evt);
		}
		catch (Exception ex)
		{
			lock (_locker)
			{
				_logger?.LogErrorInvokingHandler(ex, BrokerType, EventType, Id);
			}
		}
	}
}