using Mathematics.Statistics.Extensions;
using Utilities.Extensions;

namespace Mathematics.Statistics.Utilities;

public static class StatsUtils
{
	public static double Covariance(IList<double> samplesX, IList<double> samplesY)
	{
		ArgumentNullException.ThrowIfNull(samplesX);
		ArgumentNullException.ThrowIfNull(samplesY);

		if (samplesX == null || samplesY == null || samplesX.Count != samplesY.Count)
			throw new ArgumentException("Sample lists cannot be null and must have same length");

		if (samplesX.Count < 2)
			return 0;

		var meanX = samplesX.Average();
		var meanY = samplesY.Average();

		return samplesX.Zip(samplesY, (x, y) => (x - meanX) * (y - meanY)).Average();
	}

	public static double Correlation(IList<double> samplesX, IList<double> samplesY)
	{
		var covariance = Covariance(samplesX, samplesY);
		var stdDevX = samplesX.StandardDeviation();
		var stdDevY = samplesY.StandardDeviation();

		if (stdDevX == 0 || stdDevY == 0)
			return 0;

		return covariance / (stdDevX * stdDevY);
	}

	public static (double min, double max, double median, double q1, double q3) Summary(IList<double> samples)
	{
		ArgumentException.ThrowIfNullOrEmpty(samples);

		var sorted = samples.OrderBy(x => x).ToList();
		var min = sorted.First();
		var max = sorted.Last();
		var median = sorted.Median();

		var lowerHalf = sorted.Take(sorted.Count / 2).ToList();
		var upperHalf = sorted.Skip((sorted.Count + 1) / 2).ToList();

		var q1 = lowerHalf.Median();
		var q3 = upperHalf.Median();

		return (min, max, median, q1, q3);
	}
}