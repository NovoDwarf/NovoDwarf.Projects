using Logging.Models.Events;

namespace Logging.Models.Factories;

public static class MetricEventFactory
{
	public static CounterEvent CreateCounter(string name, double initialValue = 0) => 
		new CounterEvent { Name = name }.Increment(initialValue);

	public static StorageEvent CreateStorage(string name, double initialValue = 0) =>
		new StorageEvent { Name = name }.Increment(initialValue);
	
	public static GaugeEvent CreateGauge(string name, double value) => 
		new GaugeEvent { Name = name }.Set(value);

	public static NumericListEvent CreateNumericList(string name) => new() { Name = name };

	public static HistogramEvent CreateHistogram(string name, params double[] buckets) => new(buckets) { Name = name };
}