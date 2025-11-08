namespace Logging.Interfaces;

public interface IMetricEvent
{
	string Type { get; }
	string Name { get; }
	DateTime Timestamp { get; }

	void Reset();
	IMetricEvent MergeWith(IMetricEvent other);
}