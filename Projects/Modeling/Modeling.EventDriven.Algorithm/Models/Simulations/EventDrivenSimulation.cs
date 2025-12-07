using Messager.NET.Interfaces.Senders;
using Modeling.Core.EX;
using Modeling.DeltaT.Algorithm.Sinks;
using Modeling.EventDriven.Algorithm.Models.Nodes;
using Modeling.Logging.Models;

namespace Modeling.EventDriven.Algorithm.Models.Simulations;

public class EventDrivenSimulation : Simulation
{
	public double DeltaTime { get; } = 0.1;

	private readonly ISender<OnUpdateEvent> _tickSender;

	public EventDrivenSimulation(ISender<OnUpdateEvent> tickSender)
	{
		_tickSender = tickSender;
	}

	public override void Simulate()
	{
		InitSimulation();
		InitContext();

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
		Context = new SimulationContext(Collector);
	}

	private void CollectMetrics()
	{
		Collector.GaugeRecord("System_TotalTime", Context.CurrentTime);
		Collector.Collect();
	}

	private void Loop()
	{
		while (Context.IsRunning)
		{
			_tickSender.Send(new OnUpdateEvent { DeltaTime = DeltaTime });

			Context.Tick(DeltaTime);
		}
	}
}