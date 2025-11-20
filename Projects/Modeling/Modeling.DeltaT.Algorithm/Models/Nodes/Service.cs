using System.Diagnostics;
using Modeling.Core.Models.Abstracts.Nodes;
using Modeling.Core.Models.Abstracts.Options;
using Modeling.Core.Models.Base;

namespace Modeling.DeltaT.Algorithm.Models.Nodes;

[DebuggerDisplay("Service [{Id}]")]
public sealed class Service : ServiceBase
{
	private double _busyStart;

	private Request? _current;
	private double _serviceEnd;
	private double _serviceStart;

	public Service(ServiceOptions? options = null) : base(options)
	{
	}

	public override void Update(double deltaTime)
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

			next.Process(_current);
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
	}
}