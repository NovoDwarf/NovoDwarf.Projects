using Utilities.Extensions;

namespace Mathematics.Statistics.Extensions;

public static class ListMathExtensions
{
	extension(IList<double> samples)
	{
		public double StandardDeviation()
		{
			return Math.Sqrt(samples.Variance());
		}

		public double Variance()
		{
			ArgumentException.ThrowIfNullOrEmpty(samples);

			if (samples.Count == 1)
				return 0;

			var mean = samples.Average();

			return samples.Select(x => (x - mean) * (x - mean)).Average();
		}

		public double Skewness()
		{
			ArgumentException.ThrowIfNullOrLess(samples, 3);

			var mean = samples.Average();
			var stdDev = samples.StandardDeviation();

			if (stdDev == 0)
				return 0;

			var thirdMoment = samples.Select(x => Math.Pow((x - mean) / stdDev, 3)).Average();
			return thirdMoment;
		}

		public double Median()
		{
			ArgumentException.ThrowIfNullOrEmpty(samples);

			var sorted = samples.OrderBy(x => x).ToList();
			var count = sorted.Count;

			if (count % 2 == 0)
				return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
			return sorted[count / 2];
		}

		public double Range()
		{
			ArgumentException.ThrowIfNullOrEmpty(samples);

			return samples.Max() - samples.Min();
		}

		public double Kurtosis()
		{
			ArgumentException.ThrowIfNullOrLess(samples, 4);

			var mean = samples.Average();
			var stdDev = samples.StandardDeviation();

			if (stdDev == 0)
				return 0;

			var fourthMoment = samples.Select(x => Math.Pow((x - mean) / stdDev, 4)).Average();
			return fourthMoment - 3;
		}
	}
}