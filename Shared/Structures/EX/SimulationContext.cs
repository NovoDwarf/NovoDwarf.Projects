using Logging.Models;

namespace Structures.EX;

public class SimulationContext
{
	public SimulationContext(MetricCollector metricCollector)
	{
		Collector = metricCollector;
	}

	public MetricCollector Collector { get; }

	public double CurrentTime { get; set; }
	public double TotalTime { get; set; } = 100;

	public int Ticks { get; private set; }

	public Random Random { get; } = new();

	public bool IsRunning => CurrentTime < TotalTime;

	public void Tick(double deltaTime)
	{
		CurrentTime += deltaTime;
		Ticks++;
	}
}