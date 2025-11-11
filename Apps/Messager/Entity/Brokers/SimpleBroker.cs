using System.Collections.ObjectModel;
using Messager.Core;
using Messager.Entity.Resources;
using Messager.Extensions;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Messager.Interfaces.Services;
using Microsoft.Extensions.Logging;
using LoggerExtensions = Messager.Extensions.LoggerExtensions;

namespace Messager.Entity.Brokers;

public class SimpleBroker<TEvent> : ISender<TEvent>, IReceiver<TEvent>, IBrokerInfo
{
	private Guid Id { get; set; } = Guid.NewGuid();
	
	private readonly List<WeakAction<TEvent>> _handlers = [];
	private readonly Lock _locker = new();
	private readonly ILogger<SimpleBroker<TEvent>>? _logger;

	public SimpleBroker(ILogger<SimpleBroker<TEvent>>? logger = null)
	{
		_logger = logger;
	}

	public int SubscriberCount
	{
		get { lock (_locker) return _handlers.Count; }
	}
	
	private static string BrokerType => typeof(SimpleBroker<>).Name;
	private static string EventType => typeof(TEvent).Name;
	
	public void Send(TEvent evt)
	{
		lock (_locker)
		{
			var removedCount = _handlers.RemoveAll(s => !s.IsAlive);
			
			if (removedCount > 0) 
				_logger?.LogRemovedDeadSubscribersFromSimpleBroker(removedCount, EventType);

			var activeHandlers = _handlers.ToList();
	
			_logger?.LogSendingEvent(BrokerType, EventType, Id, activeHandlers.Count);
			
			foreach (var sub in activeHandlers)
			{
				try
				{
					sub.TryInvoke(evt);
				}
				catch (Exception ex)
				{
					_logger?.LogErrorInvokingHandler(ex,BrokerType, EventType, Id);
				}
			}
		}
	}

	public IDisposable Subscribe(Action<TEvent> handler)
	{
		lock (_locker)
		{
			_handlers.Add(new WeakAction<TEvent>(handler));
			_logger?.LogSubscriberAdded(BrokerType, EventType, Id, _handlers.Count);
		}

		return new Unsubscriber(() =>
		{
			lock (_locker)
			{
				_handlers.RemoveAll(s => s.Matches(handler));
				_logger?.LogSubscriberRemovedFromSimpleBroker(BrokerType, _handlers.Count);
			}
		});
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