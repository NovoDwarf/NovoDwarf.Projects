using Mathematics.Distributions.Models.Continuous.SemiInfinite;
using Mathematics.Statistics.Extensions;
using Mathematics.Tests.Services;

namespace Mathematics.Tests.Models.Distributions.Derived;

public class ErlangDistributionTests
{
	private const double Tolerance = 1e-10;
	private const int SampleSize = 10000;

	[Test]
	public void Constructor_WithValidParameters_SetsCorrectProperties()
	{
		const int shape = 3;
		const double rate = 2.5;

		var distribution = new ErlangDistribution(shape, rate);

		Assert.Multiple(() =>
		{
			Assert.That(distribution.Shape, Is.EqualTo(shape));
			Assert.That(distribution.Rate, Is.EqualTo(rate));
		});
	}

	[Test]
	public void Constructor_WithZeroShape_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() => new ErlangDistribution(0, 1.0));
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

		Assert.Multiple(() =>
		{
			Assert.That(erlangMean, Is.EqualTo(expectedMean).Within(0.1));
			Assert.That(expMean, Is.EqualTo(expectedMean).Within(0.1));
		});
	}

	[Test]
	public void Calculate_WithVariousShapes_ReturnsNonNegativeValues()
	{
		var shapes = new[] { 1, 2, 5, 10 };

		foreach (var shape in shapes)
		{
			var distribution = new ErlangDistribution(shape, 1.0);

			var results = TestsUtils.GenerateSamples(distribution, SampleSize);

			Assert.That(results.All(x => x >= 0), Is.True, $"All values should be non-negative for shape {shape}");
		}
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

		Assert.Multiple(() =>
		{
			Assert.That(variance5, Is.LessThan(variance1));
			Assert.That(variance10, Is.LessThan(variance5));
		});
	}

	[Test]
	public void GetExpectedValue_WithVariousParameters_ReturnsCorrectValue()
	{
		var testCases = new[]
		{
			(shape: 1, rate: 1.0, expected: 1.0),
			(shape: 2, rate: 1.0, expected: 2.0),
			(shape: 3, rate: 2.0, expected: 1.5), // 3/2 = 1.5
			(shape: 5, rate: 0.5, expected: 10.0), // 5/0.5 = 10
			(shape: 10, rate: 5.0, expected: 2.0) // 10/5 = 2
		};

		foreach (var (shape, rate, expected) in testCases)
		{
			var distribution = new ErlangDistribution(shape, rate);

			var result = distribution.GetExpectedValue();

			Assert.That(result, Is.EqualTo(expected).Within(Tolerance), $"Failed for Shape = {shape}, Rate = {rate}");
		}
	}

	[Test]
	public void GetVariance_WithVariousParameters_ReturnsCorrectValue()
	{
		var testCases = new[]
		{
			(shape: 1, rate: 1.0, expected: 1.0), // 1/(1²) = 1
			(shape: 2, rate: 1.0, expected: 2.0), // 2/(1²) = 2
			(shape: 3, rate: 2.0, expected: 0.75), // 3/(2²) = 0.75
			(shape: 5, rate: 0.5, expected: 20.0), // 5/(0.5²) = 20
			(shape: 10, rate: 5.0, expected: 0.4) // 10/(5²) = 0.4
		};

		foreach (var (shape, rate, expected) in testCases)
		{
			var distribution = new ErlangDistribution(shape, rate);

			var result = distribution.GetVariance();

			Assert.That(result, Is.EqualTo(expected).Within(Tolerance), $"Failed for shape = {shape}, rate = {rate}");
		}
	}

	[Test]
	public void GetMinValue_Always_ReturnsZero()
	{
		var distribution = new ErlangDistribution(3, 2.0);

		var result = distribution.GetMinValue();

		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void GetMaxValue_Always_ReturnsPositiveInfinity()
	{
		var distribution = new ErlangDistribution(3, 2.0);

		var result = distribution.GetMaxValue();

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_MultipleCalls_ProducesDifferentValues()
	{
		var distribution = new ErlangDistribution(2, 1.0);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++)
			results.Add(distribution.Calculate());

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

		Assert.Multiple(() =>
		{
			Assert.That(sampleMean, Is.EqualTo(expectedMean).Within(0.1));
			Assert.That(sampleVariance, Is.EqualTo(expectedVariance).Within(0.1));
		});
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
			var value = distribution.Calculate();
			var bin = (int)Math.Floor(value);
			if (bin < histogram.Length) histogram[bin]++;
		}

		Assert.That(histogram[0], Is.LessThan(histogram[1] + histogram[2]));
	}
}