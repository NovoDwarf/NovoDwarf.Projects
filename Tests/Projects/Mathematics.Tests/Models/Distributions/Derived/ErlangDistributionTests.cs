using Mathematics.Core.Extensions;
using Mathematics.Distributions.Univariate.Continuous.Semibounded;
using Mathematics.Tests.Services;

namespace Mathematics.Tests.Models.Distributions.Derived;

public class ErlangDistributionTests
{
	private const double Tolerance = 1e-10;
	private const int SampleSize = 10000;

	[Test]
	public void Constructor_WithZeroShape_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var distribution = new ErlangDistribution(0, 1.0);
		});
	}

	[Test]
	public void Constructor_WithNegativeShape_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var erlang = new ErlangDistribution(-2, 1.0);
		});
	}

	[Test]
	public void Constructor_WithZeroRate_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var erlang = new ErlangDistribution(2, 0.0);
		});
	}

	[Test]
	public void Constructor_WithNegativeRate_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var erlang = new ErlangDistribution(2, -1.5);
		});
	}

	[Test]
	public void Calculate_WithShapeOne_BehavesLikeExponential()
	{
		const double rate = 2.0;
		var erlang = new ErlangDistribution(1, rate);
		var exponential = new ExpoDistribution(rate);

		var erlangResults = TestsUtils.GenerateSamples(erlang, SampleSize);
		var expResults = TestsUtils.GenerateSamples(exponential, SampleSize);

		var erlangMean = erlangResults.Average();
		var expMean = expResults.Average();
		const double expectedMean = 1.0 / rate;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(erlangMean, Is.EqualTo(expectedMean).Within(0.1));
			Assert.That(expMean, Is.EqualTo(expectedMean).Within(0.1));
		}
	}

	[Test]
	[TestCase(1)]
	[TestCase(2)]
	[TestCase(5)]
	[TestCase(10)]
	public void Calculate_WithVariousShapes_ReturnsNonNegativeValues(int shape)
	{
		var distribution = new ErlangDistribution(shape, 1.0);

		var results = TestsUtils.GenerateSamples(distribution, SampleSize);

		Assert.That(results.All(x => x >= 0), Is.True, $"All values should be non-negative for shape {shape}");
	}

	[Test]
	public void Calculate_WithIncreasingShape_ProducesLessVariableResults()
	{
		const double rate = 1.0;
		var shape1 = new ErlangDistribution(1, rate);
		var shape5 = new ErlangDistribution(5, rate);
		var shape10 = new ErlangDistribution(10, rate);

		var results1 = TestsUtils.GenerateSamples(shape1, SampleSize);
		var results5 = TestsUtils.GenerateSamples(shape5, SampleSize);
		var results10 = TestsUtils.GenerateSamples(shape10, SampleSize);

		var variance1 = results1.Variance();
		var variance5 = results5.Variance();
		var variance10 = results10.Variance();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(variance5, Is.LessThan(variance1));
			Assert.That(variance10, Is.LessThan(variance5));
		}
	}

	[Test]
	[TestCase(1, 1.0, 1.0)]
	[TestCase(2, 1.0, 2.0)]
	[TestCase(3, 2.0, 1.5)]
	[TestCase(5, 0.5, 10.0)]
	[TestCase(10, 5.0, 2.0)]
	public void GetExpectedValue_WithVariousParameters_ReturnsCorrectValue(int shape, double rate, double expected)
	{
		var distribution = new ErlangDistribution(shape, rate);

		var result = distribution.Expected;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance), $"Failed for Shape = {shape}, Rate = {rate}");
	}

	[Test]
	[TestCase(1, 1.0, 1.0)]
	[TestCase(2, 1.0, 2.0)]
	[TestCase(3, 2.0, 0.75)]
	[TestCase(5, 0.5, 20.0)]
	[TestCase(10, 5.0, 0.4)]
	public void GetVariance_WithVariousParameters_ReturnsCorrectValue(int shape, double rate, double expected)
	{
		var distribution = new ErlangDistribution(shape, rate);

		var result = distribution.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance), $"Failed for shape = {shape}, rate = {rate}");
	}

	[Test]
	public void GetMinValue_Always_ReturnsZero()
	{
		var distribution = new ErlangDistribution(3, 2.0);

		var result = distribution.Minimum;

		Assert.That(result, Is.Zero);
	}

	[Test]
	public void GetMaxValue_Always_ReturnsPositiveInfinity()
	{
		var distribution = new ErlangDistribution(3, 2.0);

		var result = distribution.Maximum;

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_MultipleCalls_ProducesDifferentValues()
	{
		var distribution = new ErlangDistribution(2, 1.0);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++)
			results.Add(distribution.Distribute());

		Assert.That(results, Has.Count.GreaterThan(50));
	}

	[Test]
	public void Calculate_SampleMeanAndVariance_ApproximateTheoretical()
	{
		const int shape = 4;
		const double rate = 2.0;
		var distribution = new ErlangDistribution(shape, rate);

		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
		var sampleMean = samples.Average();
		var sampleVariance = samples.Variance();

		const double expectedMean = shape / rate;
		const double expectedVariance = shape / (rate * rate);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(sampleMean, Is.EqualTo(expectedMean).Within(0.1));
			Assert.That(sampleVariance, Is.EqualTo(expectedVariance).Within(0.1));
		}
	}

	[Test]
	public void Calculate_WithLargeShape_ApproachesNormalDistribution()
	{
		const int shape = 50;
		const double rate = 5.0;
		var distribution = new ErlangDistribution(shape, rate);

		var samples = TestsUtils.GenerateSamples(distribution, SampleSize);
		var skewness = samples.Skewness();

		Assert.That(Math.Abs(skewness), Is.LessThan(0.5));
	}

	[Test]
	public void Calculate_DistributionShape_CorrectForSmallShapes()
	{
		var distribution = new ErlangDistribution(2, 1.0);
		var histogram = new int[10];

		for (var i = 0; i < SampleSize; i++)
		{
			var value = distribution.Distribute();
			var bin = (int)Math.Floor(value);
			if (bin < histogram.Length) histogram[bin]++;
		}

		Assert.That(histogram[0], Is.LessThan(histogram[1] + histogram[2]));
	}
}