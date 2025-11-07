using Logging.Interfaces;
using Logging.Models.Base;

namespace Logging.Models.Events;

public sealed class HistogramEvent : BaseMetricEvent
{
	public override string Type => "histogram";
    
	private readonly double[] _buckets;
	private readonly Dictionary<double, int> _counts;

	public IReadOnlyDictionary<double, int> Counts => _counts;

	public HistogramEvent(params double[] buckets)
	{
		_buckets = buckets.OrderBy(x => x).ToArray();
		_counts = _buckets.ToDictionary(b => b, _ => 0);
	}

	public HistogramEvent Observe(double value)
	{
		var bucket = _buckets.FirstOrDefault(b => b >= value);
		_counts[bucket]++;
		UpdateTimestamp();
		return this;
	}

	public override void Reset()
	{
		foreach (var key in _counts.Keys.ToList())
			_counts[key] = 0;
	}

	public override IMetricEvent MergeWith(IMetricEvent other)
	{
		if (!CanMergeWith(other)) return this;
        
		var otherHist = (HistogramEvent)other;
		foreach (var (bucket, count) in otherHist.Counts)
			_counts[bucket] += count;
            
		UpdateTimestamp();
		return this;
	}
}