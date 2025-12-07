using Modeling.Logging.Models.Events;

namespace Modeling.Logging.Models.Factories;

public static class MetricEventFactory
{
	public static CounterEvent CreateCounter(string name, double initialValue = 0)
	{
		return new CounterEvent { Name = name }.Increment(initialValue);
	}

	public static StorageEvent CreateStorage(string name, double initialValue = 0)
	{
		return new StorageEvent { Name = name }.Increment(initialValue);
	}

	public static GaugeEvent CreateGauge(string name, double value)
	{
		return new GaugeEvent { Name = name }.Set(value);
	}

	public static NumericListEvent CreateNumericList(string name)
	{
		return new NumericListEvent { Name = name };
	}

	public static HistogramEvent CreateHistogram(string name, params double[] buckets)
	{
		return new HistogramEvent(buckets) { Name = name };
	}
}