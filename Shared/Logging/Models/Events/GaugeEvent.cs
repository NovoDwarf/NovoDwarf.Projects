using Logging.Interfaces;
using Logging.Models.Base;

namespace Logging.Models.Events;

public sealed class GaugeEvent : BaseMetricEvent
{
	public override string Type => "gauge";
	public double Value { get; private set; }

	public GaugeEvent Set(double value)
	{
		Value = value;
		UpdateTimestamp();
		return this;
	}

	public override void Reset()
	{
		Value = 0;
	}

	public override IMetricEvent MergeWith(IMetricEvent other)
	{
		return !CanMergeWith(other)
			? this
			: Set(((GaugeEvent)other).Value);
	}
}