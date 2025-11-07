using Logging.Interfaces;

namespace Logging.Models.Managers;

public class SinkManager
{
	private readonly List<IMetricSink> _sinks = [];
	private readonly Lock _lock = new();

	public void AddSink(IMetricSink sink)
	{
		lock (_lock) _sinks.Add(sink);
	}

	public void RemoveSink(IMetricSink sink)
	{
		lock (_lock) _sinks.Remove(sink);
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