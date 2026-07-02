using System.Diagnostics;
using Modeling.Core.EX;
using Modeling.Core.Models.Abstracts.Nodes;
using Modeling.Core.Models.Abstracts.Options;
using Modeling.Core.Models.Base;
using NovoDwarf.Modeling.EventDriven.Models.Simulations;

namespace NovoDwarf.Modeling.EventDriven.Models.Nodes;

[DebuggerDisplay("Service [{Id}]")]
public sealed class Service : ServiceBase
{
	private readonly EventCollector _collector;
	private readonly Dictionary<Guid, double> _arrivalTimes = new();

	private double _busyStart;
	private Request? _current;
	private double _serviceEnd;
	private double _serviceStart;

	public Service(EventCollector collector, ServiceOptions? options = null)
		: base(options)
	{
		_collector = collector;
	}

	public override void Process(Request request)
	{
		Enqueue(request);
		
		_arrivalTimes[request.Id] = Context.CurrentTime;
		
		Context.Collector.GaugeRecord($"{Id}_Service_Queue_Size", Storage.Count);
		Context.Collector.ListAdd($"{Id}_Service_Queue_Size_History", Storage.Count);

		TryStartService();
	}

	public override void Update(double deltaTime) { }

	private void TryStartService()
	{
		if (_current != null)
			return;

		if (IsEmpty)
			return;

		_current = Dequeue();

		_serviceStart = Context.CurrentTime;
		_serviceEnd = Context.CurrentTime + DoService();

		_busyStart = Context.CurrentTime;
		IsBusy = true;

		if (_current != null && _arrivalTimes.TryGetValue(_current.Id, out var enterTime))
		{
			var waitTime = Context.CurrentTime - enterTime;
			Context.Collector.ListAdd($"{Id}_Service_WaitTime", waitTime);
			_arrivalTimes.Remove(_current.Id);
		}

		_collector.Enqueue(new SimulationEvent(_serviceEnd, (ctx, collector) => CompleteService(ctx)));

		Context.Collector.GaugeRecord($"{Id}_Service_IsBusy", 1);
	}

	private double DoService()
	{
		return Math.Max(0, Distribution.Distribute());
	}

	private void CompleteService(SimulationContext ctx)
	{
		ctx.CurrentTime = _serviceEnd;

		var serviceDuration = ctx.CurrentTime - _serviceStart;

		if (_current != null)
		{
			ctx.Collector.CounterIncrement($"{Id}_Service_Completed");
			ctx.Collector.ListAdd($"{Id}_Service_Duration", serviceDuration);

			EndBusyPeriod(ctx.CurrentTime);

			var next = GetAvailableExit();
			var finished = _current;
			_current = null;

			if (next != null && finished != null)
				next.Process(finished);
		}

		TryStartService();

		foreach (var input in Inputs.OfType<Queue>())
			input.TrySendNext();
	}

	private void EndBusyPeriod(double currentTime)
	{
		if (!IsBusy)
			return;

		var busyTime = currentTime - _busyStart;
		
		Context.Collector.ListAdd($"{Id}_Service_BusyTime", busyTime);
		IsBusy = false;
	}
}