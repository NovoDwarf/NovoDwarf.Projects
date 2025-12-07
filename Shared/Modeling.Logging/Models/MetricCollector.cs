using Modeling.Logging.Interfaces;
using Modeling.Logging.Models.Factories;
using Modeling.Logging.Models.Managers;
using Modeling.Logging.Models.Storages;

namespace Modeling.Logging.Models;

public sealed class MetricCollector
{
	private readonly SinkManager _sinkManager;
	private readonly IMetricStorage _storage;

	public MetricCollector(IMetricStorage? storage = null, SinkManager? sinkManager = null)
	{
		_storage = storage ?? new InMemoryStorage();
		_sinkManager = sinkManager ?? new SinkManager();
	}

	public void CounterIncrement(string name, double value = 1)
	{
		_storage.Store(MetricEventFactory.CreateCounter(name, value));
	}

	public void GaugeRecord(string name, double value)
	{
		_storage.Store(MetricEventFactory.CreateGauge(name, value));
	}

	public void ListAdd(string name, double value)
	{
		_storage.Store(MetricEventFactory.CreateNumericList(name).Add(value));
	}

	public void HistogramRecord(string name, double value, params double[] buckets)
	{
		_storage.Store(MetricEventFactory.CreateHistogram(name, buckets).Observe(value));
	}

	public void AddSink(IMetricSink sink)
	{
		_sinkManager.AddSink(sink);
	}

	public Report Collect()
	{
		var report = new Report(_storage.GetAll(), DateTime.UtcNow);

		_sinkManager.FlushAll(report);

		return report;
	}

	public void Reset()
	{
		_storage.Clear();
	}
}