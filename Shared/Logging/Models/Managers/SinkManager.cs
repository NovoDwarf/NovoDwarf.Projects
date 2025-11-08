using Logging.Interfaces;

namespace Logging.Models.Managers;

public class SinkManager
{
	private readonly Lock _lock = new();
	private readonly List<IMetricSink> _sinks = [];

	public void AddSink(IMetricSink sink)
	{
		lock (_lock)
		{
			_sinks.Add(sink);
		}
	}

	public void RemoveSink(IMetricSink sink)
	{
		lock (_lock)
		{
			_sinks.Remove(sink);
		}
	}

	public void FlushAll(Report report)
	{
		lock (_lock)
		{
			foreach (var sink in _sinks)
				sink.Flush(report);
		}
	}
}