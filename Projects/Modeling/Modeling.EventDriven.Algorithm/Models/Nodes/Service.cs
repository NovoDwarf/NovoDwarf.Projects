using System.Diagnostics;
using Messager.NET.Interfaces.Receivers;
using Messager.NET.Interfaces.Senders;
using Modeling.Core.Models.Abstracts.Nodes;
using Modeling.Core.Models.Abstracts.Options;
using Modeling.Core.Models.Base;

namespace Modeling.EventDriven.Algorithm.Models.Nodes;

[DebuggerDisplay("Service [{Id}]")]
public sealed class Service : ServiceBase, IDisposable
{
	private double _busyStart;
	private Request? _current;
	private double _serviceEnd;
	private double _serviceStart;
	
	private readonly ISender<OnUpdateEvent> _tickSender;
	private readonly ISender<OnProcessEvent> _processSender;
	private readonly IReceiver<OnUpdateEvent> _tickReceiver;
	private readonly IReceiver<OnProcessEvent> _processReceiver;
	
	private IDisposable? _tickSubscription;
	private IDisposable? _processSubscription;

	public Service(ServiceOptions? options = null, 
		ISender<OnUpdateEvent>? tickSender = null,
		ISender<OnProcessEvent>? processSender = null,
		IReceiver<OnUpdateEvent>? tickReceiver = null,
		IReceiver<OnProcessEvent>? processReceiver = null) 
		: base(options)
	{
		_tickSender = tickSender ?? throw new ArgumentNullException(nameof(tickSender));
		_processSender = processSender ?? throw new ArgumentNullException(nameof(processSender));
		_tickReceiver = tickReceiver ?? throw new ArgumentNullException(nameof(tickReceiver));
		_processReceiver = processReceiver ?? throw new ArgumentNullException(nameof(processReceiver));
		
		SubscribeToEvents();
	}

	private void SubscribeToEvents()
	{
		_tickSubscription = _tickReceiver.Subscribe(OnTick);
		_processSubscription = _processReceiver.Subscribe(OnProcess);
	}

	private void OnTick(OnUpdateEvent evt)
	{
		if (IsServiceComplete())
			CompleteService();

		if (CanStartNewService())
			StartService();

		if (IsBusy)
			Context.Collector.GaugeRecord($"{Id}_Service_IsBusy", 1);
		else
			Context.Collector.GaugeRecord($"{Id}_Service_IsBusy", 0);
	}

	private void OnProcess(OnProcessEvent evt)
	{
		// Обработка входящих запросов, если необходимо
		// Например, можно добавить логику обработки входящих ProcessEvent'ов
	}

	private bool IsServiceComplete()
	{
		return _current != null && Context.CurrentTime >= _serviceEnd;
	}

	private bool CanStartNewService()
	{
		return _current == null && Storage.Count > 0;
	}

	private double DoService()
	{
		return Math.Max(0, Distribution.Distribute());
	}

	private void CompleteService()
	{
		var serviceDuration = Context.CurrentTime - _serviceStart;

		if (_current != null)
		{
			//_current.ServiceTime += serviceDuration;
			Context.Collector.CounterIncrement($"{Id}_Service_Completed");
			Context.Collector.ListAdd($"{Id}_Service_Duration", serviceDuration);

			EndBusyPeriod(Context.CurrentTime);

			var next = GetAvailableExit();

			if (next == null)
				return;

			// Отправляем событие обработки вместо прямого вызова
			var processEvent = new OnProcessEvent 
			{ 
				Request = _current,
				NodeId = Id,
				Timestamp = Context.CurrentTime
			};
			_processSender.Send(processEvent);
		}

		_current = null;
	}

	private void EndBusyPeriod(double currentTime)
	{
		if (!IsBusy)
			return;

		var busyTime = currentTime - _busyStart;
		Context.Collector.ListAdd($"{Id}_Service_BusyTime", busyTime);
		IsBusy = false;
	}

	private void StartService()
	{
		_current = Dequeue();

		_serviceStart = Context.CurrentTime;
		_serviceEnd = Context.CurrentTime + DoService();

		_busyStart = Context.CurrentTime;
		IsBusy = true;

		var startEvent = new OnProcessEvent 
		{ 
			Request = _current,
			NodeId = Id,
			Timestamp = Context.CurrentTime,
		};
		_processSender.Send(startEvent);
	}

	public void Dispose()
	{
		_tickSubscription?.Dispose();
		_processSubscription?.Dispose();
	}
}

// Примеры классов событий (должны быть определены в вашем проекте)
public class OnUpdateEvent
{
	public double Timestamp { get; set; }
	public double DeltaTime { get; set; }
}

public class OnProcessEvent
{
	public Request? Request { get; set; }
	public Guid NodeId { get; set; }
	public double Timestamp { get; set; }
}