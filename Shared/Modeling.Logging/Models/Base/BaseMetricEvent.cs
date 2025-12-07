using Modeling.Logging.Interfaces;

namespace Modeling.Logging.Models.Base;

public abstract class BaseMetricEvent : IMetricEvent
{
	public required string Name { get; init; }
	public DateTime Timestamp { get; protected set; } = DateTime.UtcNow;
	public abstract string Type { get; }

	public abstract void Reset();
	public abstract IMetricEvent MergeWith(IMetricEvent other);

	protected void UpdateTimestamp()
	{
		Timestamp = DateTime.UtcNow;
	}

	protected bool CanMergeWith(IMetricEvent other)
	{
		return other?.GetType() == GetType() && other.Name == Name;
	}
}