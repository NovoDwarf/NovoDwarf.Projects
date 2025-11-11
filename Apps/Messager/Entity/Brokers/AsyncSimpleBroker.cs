using Messager.Entity.Resources;
using Messager.Extensions;
using Messager.Interfaces.Core;
using Messager.Interfaces.Receivers;
using Messager.Interfaces.Senders;
using Microsoft.Extensions.Logging;

namespace Messager.Entity.Brokers;

public class AsyncSimpleBroker<TEvent> : IBroker<TEvent>, IAsyncSender<TEvent>, IAsyncReceiver<TEvent>
{
	private readonly List<Func<TEvent, ValueTask>> _handlers = [];
	private readonly Lock _locker = new();
	private readonly ILogger<AsyncSimpleBroker<TEvent>>? _logger;

	public AsyncSimpleBroker(ILogger<AsyncSimpleBroker<TEvent>>? logger = null)
	{
		_logger = logger;
	}
	
	public Guid Id { get; set; } = Guid.NewGuid();
	
	public string BrokerType => typeof(SimpleBroker<>).Name;
	public string EventType => typeof(TEvent).Name;

	public IAsyncDisposable Subscribe(Func<TEvent, ValueTask> handler)
	{
		lock (_locker)
		{
			_handlers.Add(handler);
			_logger?.LogSubscriberAdded(BrokerType, EventType, Id);
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
			var removed = _handlers.Remove(handler);
			if (removed)
			{
				_logger?.LogDebug("Unsubscribed handler from AsyncSimpleBroker<{EventType}>. Remaining subscribers: {Count}", typeof(TEvent).Name, _handlers.Count);
			}
		}
	}

	public async ValueTask SendAsync(TEvent evt)
	{
		List<Func<TEvent, ValueTask>> handlersCopy;
		
		lock (_locker)
		{
			handlersCopy = _handlers.ToList();
			_logger?.LogTrace("Sending async event {EventType} to {Count} subscribers", typeof(TEvent).Name, handlersCopy.Count);
		}

		foreach (var handler in handlersCopy)
		{
			try
			{
				await handler(evt);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Error invoking async handler for event {EventType}", typeof(TEvent).Name);
			}
		}
	}
}