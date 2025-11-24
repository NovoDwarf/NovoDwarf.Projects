using Mathematics.Core.Extensions;
using Mathematics.Distributions.Univariate.Continuous.Unbounded;
using Mathematics.Tests.Services;

namespace Mathematics.Tests.Models.Distributions.Basic;

[TestFixture]
public class NormalDistributionTests
{
	private const double Tolerance = 1e-10;
	private const int SampleSize = 10000;

	[Test]
	public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
	{
		const double mean = 5.0;
		const double stdDev = 2.0;

		var distribution = new NormalDistribution(mean, stdDev);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(distribution.Mean, Is.EqualTo(mean));
			Assert.That(distribution.StandardDeviation, Is.EqualTo(stdDev));
		}
	}

	[Test]
	public void Constructor_WithZeroStandardDeviation_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var distribution = new NormalDistribution(0, 0);
		});
	}

	[Test]
	public void Constructor_WithNegativeStandardDeviation_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var distribution = new NormalDistribution(0, -1.5);
		});
	}

	[Test]
	public void Calculate_StandardNormal_ProducesZeroMeanAndUnitVariance()
	{
		var distribution = new NormalDistribution(0, 1);

		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
		var sampleMean = samples.Average();
		var sampleVariance = samples.Variance();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(sampleMean, Is.EqualTo(0).Within(0.1));
			Assert.That(sampleVariance, Is.EqualTo(1).Within(0.1));
		}
	}

	[Test]
	[TestCase(-2.0, 1.0)]
	[TestCase(0.0, 1.0)]
	[TestCase(5.0, 1.0)]
	[TestCase(10.0, 1.0)]
	public void Calculate_WithDifferentMeans_ShiftsDistribution(double mean, double stdDev)
	{
		var distribution = new NormalDistribution(mean, stdDev);

		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
		var sampleMean = samples.Average();

		Assert.That(sampleMean, Is.EqualTo(mean).Within(0.1), $"Failed for mean = {mean}");
	}

	[Test]
	public void Calculate_WithDifferentStandardDeviations_ScalesDistribution()
	{
		const double mean = 0.0;
		var stdDevs = new[] { 0.5, 1.0, 2.0, 5.0 };

		foreach (var stdDev in stdDevs)
		{
			var distribution = new NormalDistribution(mean, stdDev);

			var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
			var sampleVariance = samples.Variance();
			var expectedVariance = stdDev * stdDev;


			Assert.That(sampleVariance, Is.EqualTo(expectedVariance).Within(0.1), $"Failed for stdDev = {stdDev}");
		}
	}

	[Test]
	[TestCase(-5.0, 1.0)]
	[TestCase(0.0, 2.0)]
	[TestCase(10.0, 0.5)]
	[TestCase(100.0, 10.0)]
	public void GetExpectedValue_Always_ReturnsMean(double mean, double stdDev)
	{
		var distribution = new NormalDistribution(mean, stdDev);

		var result = distribution.Expected;

		Assert.That(result, Is.EqualTo(mean).Within(Tolerance));
	}

	[Test]
	[TestCase(0.0, 1.0, 1.0)]
	[TestCase(0.0, 2.0, 4.0)]
	[TestCase(5.0, 0.5, 0.25)]
	[TestCase(-2.0, 3.0, 9.0)]
	public void GetVariance_Always_ReturnsSquaredStandardDeviation(double mean, double stdDev, double expected)
	{
		var distribution = new NormalDistribution(mean, stdDev);

		var result = distribution.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetMinValue_Always_ReturnsNegativeInfinity()
	{
		var distribution = new NormalDistribution(0, 1);

		var result = distribution.Minimum;

		Assert.That(result, Is.EqualTo(double.NegativeInfinity));
	}

	[Test]
	public void GetMaxValue_Always_ReturnsPositiveInfinity()
	{
		var distribution = new NormalDistribution(0, 1);

		var result = distribution.Maximum;

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_DistributionShape_ApproximatelyNormal()
	{
		var distribution = new NormalDistribution(0, 1);
		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);

		var skewness = samples.Skewness();
		var kurtosis = samples.Kurtosis();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(skewness, Is.EqualTo(0).Within(0.1));
			Assert.That(kurtosis, Is.EqualTo(0).Within(0.5));
		}
	}

	[Test]
	public void Calculate_EmpiricalRule_68_95_99_7()
	{
		var distribution = new NormalDistribution(0, 1);
		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);

		var within1Std = samples.Count(x => Math.Abs(x) <= 1) / (double)SampleSize;
		var within2Std = samples.Count(x => Math.Abs(x) <= 2) / (double)SampleSize;
		var within3Std = samples.Count(x => Math.Abs(x) <= 3) / (double)SampleSize;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(within1Std, Is.EqualTo(0.6827).Within(0.02));
			Assert.That(within2Std, Is.EqualTo(0.9545).Within(0.02));
			Assert.That(within3Std, Is.EqualTo(0.9973).Within(0.01));
		}
	}

	[Test]
	public void Calculate_MultipleCalls_ProducesDifferentValues()
	{
		var distribution = new NormalDistribution(0, 1);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++)
			results.Add(distribution.Distribute());

		Assert.That(results, Has.Count.GreaterThan(50));
	}

	[Test]
	public void Calculate_WithExtremeParameters_ProducesValidNumbers()
	{
		var distribution = new NormalDistribution(1e6, 1e3);

		var samples = TestsUtils.GenerateSamples(distribution, 1000);

		Assert.That(samples.All(x => !double.IsInfinity(x) && !double.IsNaN(x)), Is.True);
	}

	[Test]
	public void Calculate_SampleQuantiles_MatchNormalDistribution()
	{
		var distribution = new NormalDistribution(0, 1);
		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
		var sorted = samples.OrderBy(x => x).ToList();

		var median = sorted.Median();
		var q1 = sorted[SampleSize / 4];
		var q3 = sorted[3 * SampleSize / 4];

		Assert.That(median, Is.EqualTo(0).Within(0.1));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(q1, Is.EqualTo(-0.6745).Within(0.1));
			Assert.That(q3, Is.EqualTo(0.6745).Within(0.1));
		}
	}
}