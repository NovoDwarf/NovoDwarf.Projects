using Mathematics.Core.Distributions.Models.Univariate.Continuous.Bounded;
using Mathematics.Core.Statistics.Extensions;
using Mathematics.Tests.Services;

namespace Mathematics.Tests.Models.Distributions.Basic;

[TestFixture]
public class UniformDistributionTests
{
	private const double Tolerance = 1e-10;
	private const int SampleSize = 10000;

	[Test]
	public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
	{
		const double min = 2.0;
		const double max = 5.0;

		var distribution = new UniformDistribution(min, max);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(distribution.Minimum, Is.EqualTo(min));
			Assert.That(distribution.Maximum, Is.EqualTo(max));
		}
	}

	[Test]
	public void Constructor_WithMinEqualToMax_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() => new UniformDistribution(5.0, 5.0));
	}

	[Test]
	public void Constructor_WithMinGreaterThanMax_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() => new UniformDistribution(10.0, 5.0));
	}

	[Test]
	public void Calculate_StandardUniform_ProducesValuesInZeroToOneRange()
	{
		var distribution = new UniformDistribution(0, 1);

		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(samples.All(x => x is >= 0 and < 1), Is.True);
			Assert.That(samples.Min(), Is.GreaterThanOrEqualTo(0));
			Assert.That(samples.Max(), Is.LessThan(1));
		}
	}

	[Test]
	[TestCase(-5.0, 5.0)]
	[TestCase(10.0, 20.0)]
	[TestCase(-100.0, -50.0)]
	[TestCase(0.0, 0.001)]
	public void Calculate_WithDifferentRanges_RespectsBounds(double min, double max)
	{
		var distribution = new UniformDistribution(min, max);

		var samples = TestsUtils.GenerateSamples(distribution, 1000);

		Assert.That(samples.All(x => x >= min && x < max), Is.True, $"Failed for range [{min}, {max})");
	}

	[Test]
	public void Calculate_ValuesAreUniformlyDistributed()
	{
		var distribution = new UniformDistribution(0, 100);
		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
		var histogram = new int[10];

		foreach (var sample in samples)
		{
			var bin = (int)(sample / 10);

			if (bin >= 0 && bin < histogram.Length)
				histogram[bin]++;
		}

		var expectedCount = SampleSize / histogram.Length;

		foreach (var count in
		         histogram) Assert.That(count, Is.EqualTo(expectedCount).Within(expectedCount * 0.15)); // ±15%
	}

	[Test]
	[TestCase(0.0, 1.0, 0.5)]
	[TestCase(-5.0, 5.0, 0.0)]
	[TestCase(10.0, 20.0, 15.0)]
	[TestCase(-10.0, 0.0, -5.0)]
	[TestCase(2.5, 7.5, 5.0)]
	public void GetExpectedValue_WithVariousRanges_ReturnsCorrectMean(double min, double max, double expected)
	{
		var distribution = new UniformDistribution(min, max);

		var result = distribution.Expected;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance), $"Failed for range [{min}, {max})");
	}

	[Test]
	[TestCase(0.0, 1.0, 1.0 / 12.0)]
	[TestCase(0.0, 2.0, 4.0 / 12.0)]
	[TestCase(-1.0, 1.0, 4.0 / 12.0)]
	[TestCase(5.0, 10.0, 25.0 / 12.0)]
	[TestCase(-3.0, 3.0, 36.0 / 12.0)]
	public void GetVariance_WithVariousRanges_ReturnsCorrectValue(double min, double max, double expected)
	{
		var distribution = new UniformDistribution(min, max);

		var result = distribution.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance), $"Failed for range [{min}, {max})");
	}

	[Test]
	public void GetMinValue_Always_ReturnsMin()
	{
		var distribution = new UniformDistribution(2.5, 7.5);

		var result = distribution.Minimum;

		Assert.That(result, Is.EqualTo(2.5));
	}

	[Test]
	public void GetMaxValue_Always_ReturnsMax()
	{
		var distribution = new UniformDistribution(2.5, 7.5);

		var result = distribution.Maximum;

		Assert.That(result, Is.EqualTo(7.5));
	}

	[Test]
	public void ToString_WithParameters_ReturnsFormattedString()
	{
		var distribution = new UniformDistribution(2.5, 7.5);

		var result = distribution.ToString();

		Assert.That(result, Is.EqualTo("Uniform [Min = 2.500, Max = 7.500)"));
	}

	[Test]
	public void Calculate_SampleMeanAndVariance_ApproximateTheoretical()
	{
		const double min = 2.0;
		const double max = 8.0;
		var distribution = new UniformDistribution(min, max);

		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
		var sampleMean = samples.Average();
		var sampleVariance = samples.Variance();

		const double expectedMean = (min + max) / 2.0;
		var expectedVariance = Math.Pow(max - min, 2) / 12.0;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(sampleMean, Is.EqualTo(expectedMean).Within(0.05));
			Assert.That(sampleVariance, Is.EqualTo(expectedVariance).Within(0.05));
		}
	}

	[Test]
	public void Calculate_NoValuesOutsideRange()
	{
		var distribution = new UniformDistribution(-10, 10);

		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(samples.Any(x => x < -10), Is.False);
			Assert.That(samples.Any(x => x >= 10), Is.False);
		}
	}

	[Test]
	public void Calculate_DistributionIsFlat_NoSkewness()
	{
		var distribution = new UniformDistribution(0, 1);
		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);

		var skewness = samples.Skewness();

		Assert.That(skewness, Is.EqualTo(0).Within(0.1));
	}

	[Test]
	public void Calculate_WithVerySmallRange_WorksCorrectly()
	{
		var distribution = new UniformDistribution(0.499, 0.501);

		var samples = TestsUtils.GenerateSamples(distribution, 1000);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(samples.All(x => x is >= 0.499 and < 0.501), Is.True);
			Assert.That(samples.Min(), Is.GreaterThanOrEqualTo(0.499));
			Assert.That(samples.Max(), Is.LessThan(0.501));
		}
	}

	[Test]
	public void Calculate_WithNegativeRange_WorksCorrectly()
	{
		var distribution = new UniformDistribution(-100, -50);

		var samples = TestsUtils.GenerateSamples(distribution, 1000);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(samples.All(x => x is >= -100 and < -50), Is.True);
			Assert.That(samples.Average(), Is.EqualTo(-75).Within(1.0));
		}
	}

	[Test]
	public void Calculate_MultipleCalls_ProducesDifferentValues()
	{
		var distribution = new UniformDistribution(0, 100);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++)
			results.Add(distribution.Distribute());

		Assert.That(results, Has.Count.GreaterThan(50));
	}

	[Test]
	public void Calculate_EdgeCaseValues_ApproachBoundsButNotEqual()
	{
		var distribution = new UniformDistribution(0, 1);
		const int attempts = 10000;
		var gotVeryCloseToMax = false;

		for (var i = 0; i < attempts; i++)
		{
			var value = distribution.Distribute();

			if (value > 0.999)
				gotVeryCloseToMax = true;

			Assert.That(value, Is.LessThan(1.0));
		}

		Assert.That(gotVeryCloseToMax, Is.True);
	}
}