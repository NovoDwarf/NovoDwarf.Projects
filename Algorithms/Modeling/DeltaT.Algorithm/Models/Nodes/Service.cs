using System.Diagnostics;
using Structures.Models.Abstracts.Nodes;
using Structures.Models.Abstracts.Options;
using Structures.Models.Base;

namespace DeltaT.Algorithm.Models.Nodes;

[DebuggerDisplay("Service [{Id}]")]
public sealed class Service : ServiceBase
{
	public Service(ServiceOptions? options = null) : base(options) { }
	
	private Request? _current;

	private double _busyStart;
	private double _serviceEnd;
	private double _serviceStart;
	
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

	private bool IsServiceComplete() => _current != null && Context.CurrentTime >= _serviceEnd;
	private bool CanStartNewService() => _current == null && Storage.Count > 0;
	private double DoService() => Math.Max(0, Distribution.Calculate());
	
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