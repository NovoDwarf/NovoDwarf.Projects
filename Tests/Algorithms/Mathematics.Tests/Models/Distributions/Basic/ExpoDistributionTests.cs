using Mathematics.Distributions.Models.Continuous.SemiInfinite;
using Mathematics.Tests.Services;

namespace Mathematics.Tests.Models.Distributions.Basic;

[TestFixture]
public class ExpoDistributionTests
{
	private const double Tolerance = 1e-10;
	private const int SampleSize = 10000;

	[Test]
	[TestCase(2.5)]
	[TestCase(1.0)]
	[TestCase(0.5)]
	public void Constructor_WithPositiveRate_SetsCorrectRate(double rate)
	{

		var distribution = new ExpoDistribution(rate);

		Assert.That(distribution.Rates, Is.EqualTo(rate));
	}

	[Test]
	public void Constructor_WithZeroRate_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var exponential = new ExpoDistribution(0);
		});
	}

	[Test]
	public void Constructor_WithNegativeRate_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var exponential = new ExpoDistribution(-1.5);
		});
	}

	[Test]
	public void Calculate_WithRateOne_ReturnsNonNegativeValues()
	{
		var distribution = new ExpoDistribution(1.0);

		var results = TestsUtils.GenerateSamples(distribution, SampleSize);

		Assert.That(results.All(x => x >= 0), Is.True);
	}

	[Test]
	[TestCase(0.5)]
	[TestCase(1.0)]
	[TestCase(2.0)]
	[TestCase(5.0)]
	public void Calculate_WithDifferentRates_RespectsRateParameter(double rate)
	{
		var distribution = new ExpoDistribution(rate);

		var mean = TestsUtils.GenerateSamples(distribution, SampleSize).Average();

		var expectedMean = 1.0 / rate;
		Assert.That(mean, Is.EqualTo(expectedMean).Within(0.1));
		
	}

	[Test]
	[TestCase(0.5, 2.0)]
	[TestCase(1.0, 1.0)]
	[TestCase(2.0, 0.5)]
	[TestCase(10.0, 0.1)]
	public void GetExpectedValue_WithVariousRates_ReturnsCorrectValue(double rate, double expected)
	{
		var distribution = new ExpoDistribution(rate);

		var result = distribution.GetExpectedValue();
			
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	[TestCase(0.5, 4.0)]
	[TestCase(1.0, 1.0)]
	[TestCase(2.0, 0.25)]
	[TestCase(4.0, 0.0625)]
	public void GetVariance_WithVariousRates_ReturnsCorrectValue(double rate, double expected)
	{
		var distribution = new ExpoDistribution(rate);

		var result = distribution.GetVariance();
			
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetMinValue_Always_ReturnsZero()
	{
		var distribution = new ExpoDistribution(1.0);

		var result = distribution.GetMinValue();

		Assert.That(result, Is.Zero);
	}

	[Test]
	public void GetMaxValue_Always_ReturnsPositiveInfinity()
	{
		var distribution = new ExpoDistribution(1.0);

		var result = distribution.GetMaxValue();

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_MultipleCalls_ProducesDifferentValues()
	{
		var distribution = new ExpoDistribution(1.0);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++)
			results.Add(distribution.Calculate());

		Assert.That(results, Has.Count.GreaterThan(50));
	}

	[Test]
	public void Calculate_WithHighRate_ProducesSmallValues()
	{
		var distribution = new ExpoDistribution(1000.0);

		var results = TestsUtils.GenerateSamples(distribution, 1000);

		Assert.That(results.All(x => x < 0.01), Is.True);
	}

	[Test]
	public void Calculate_WithLowRate_ProducesLargeValues()
	{
		var distribution = new ExpoDistribution(0.01);

		var results = TestsUtils.GenerateSamples(distribution, 1000);

        using (Assert.EnterMultipleScope())
        {
			Assert.That(results.All(x => x > 1.0), Is.False);
			Assert.That(results.Any(x => x > 100.0), Is.True);
		}
	}

	[Test]
	public void Calculate_DistributionShape_ApproximatesExponential()
	{
		var distribution = new ExpoDistribution(1.0);
		var histogram = new int[10];

		for (var i = 0; i < SampleSize; i++)
		{
			var value = distribution.Calculate();
			var bin = (int)Math.Floor(value);
			
			if (bin < histogram.Length) 
				histogram[bin]++;
		}

		for (var i = 1; i < histogram.Length - 1; i++) Assert.That(histogram[i], Is.LessThan(histogram[i - 1] * 1.5));
	}
}