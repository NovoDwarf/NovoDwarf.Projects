using Modeling.Core.EX;
using Modeling.DeltaT.Algorithm.Sinks;
using NovoDwarf.Modeling.Logging.Models;

namespace NovoDwarf.Modeling.DeltaT.Models.Simulations;

public class SequentialSimulation : Simulation
{
	public double DeltaTime { get; } = 0.1;

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
			foreach (var node in Nodes) 
				node.Update(DeltaTime);

			Context.Tick(DeltaTime);
		}
	}
}