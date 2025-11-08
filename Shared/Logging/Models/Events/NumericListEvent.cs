using Logging.Interfaces;
using Logging.Models.Base;

namespace Logging.Models.Events;

public sealed class NumericListEvent : BaseMetricEvent
{
	public override string Type => "numeric_list";
	public List<double> Values { get; } = new();

	public int Count => Values.Count;
	public double Sum => Values.Sum();
	public double Average => Count > 0 ? Values.Average() : 0;
	public double Min => Count > 0 ? Values.Min() : 0;
	public double Max => Count > 0 ? Values.Max() : 0;

	public NumericListEvent Add(double value)
	{
		Values.Add(value);
		UpdateTimestamp();
		return this;
	}

	public override void Reset()
	{
		Values.Clear();
	}

	public override IMetricEvent MergeWith(IMetricEvent other)
	{
		if (!CanMergeWith(other)) return this;

		var otherList = (NumericListEvent)other;
		Values.AddRange(otherList.Values);
		UpdateTimestamp();
		return this;
	}
}