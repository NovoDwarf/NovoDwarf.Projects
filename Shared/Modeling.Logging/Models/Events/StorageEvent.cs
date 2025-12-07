using Modeling.Logging.Interfaces;
using Modeling.Logging.Models.Base;

namespace Modeling.Logging.Models.Events;

public class StorageEvent : BaseMetricEvent
{
	public override string Type => "storage";
	public double Value { get; private set; }

	public StorageEvent Increment(double value = 1)
	{
		Value += value;
		UpdateTimestamp();
		return this;
	}

	public override void Reset()
	{
		Value = 0;
	}

	public override IMetricEvent MergeWith(IMetricEvent other)
	{
		if (!CanMergeWith(other)) return this;
		Value += ((CounterEvent)other).Value;
		UpdateTimestamp();
		return this;
	}
}