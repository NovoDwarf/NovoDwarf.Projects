using Modeling.Core.EX;
using Modeling.DeltaT.Algorithm.Sinks;
using Modeling.EventDriven.Algorithm.Models.Nodes;
using Modeling.Logging.Models;

namespace Modeling.EventDriven.Algorithm.Models.Simulations;

public class EventDrivenSimulation : Simulation
{
	public EventCollector EventCollector { get; } = new();

	public override void Simulate()
	{
		InitSimulation();
		InitContext();
		ScheduleInitialEvents();
		Loop();
		CollectMetrics();
	}

	private void InitContext()
	{
		foreach (var node in Nodes) node.SetContext(Context);
	}

	private void InitSimulation()
	{
		Collector = new MetricCollector();
		Collector.AddSink(new StatisticsSink());
		
		Context = new SimulationContext(Collector)
		{
			CompleteRequest = CompleteRequest
		};
	}
	
	private void ScheduleInitialEvents()
	{
		foreach (var source in Nodes.OfType<Source>())
		{
			var firstTime = Context.CurrentTime + source.NextEventTime;
			ScheduleSourceEvent(source, firstTime);
		}
	}

	private void ScheduleSourceEvent(Source source, double time)
	{
		EventCollector.Enqueue(new SimulationEvent(
			time,
			(ctx, collector) =>
			{
				ctx.CurrentTime = time;
				source.Update(0);

				var nextTime = ctx.CurrentTime + source.NextEventTime;

				if (double.IsFinite(nextTime) && nextTime > ctx.CurrentTime)
					ScheduleSourceEvent(source, nextTime);
			}));
	}
	
	private void CollectMetrics()
	{
		Collector.GaugeRecord("System_TotalTime", Context.CurrentTime);
		Collector.Collect();
	}

	private void Loop()
	{
		while (Context.IsRunning && EventCollector.Count > 0)
		{
			if (!EventCollector.TryDequeue(out var ev) || ev == null) 
				continue;
			
			ev.Execute(Context, EventCollector);
		}
	}
}