using Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

namespace Mathematics.Tests.Models.Distributions.HeavyTailed;

[TestFixture]
public class LevyDistributionTests
{
	private const int SampleSize = 100000;
	
	[Test]
	[TestCase(5.0, 2.0)]
	public void Calculate_RespectsLocationParameter(double location, double scale)
	{
		var levy = new LevyDistribution(location, scale);

		for (var i = 0; i < 100; i++)
		{
			var result = levy.Distribute();
			Assert.That(result, Is.GreaterThanOrEqualTo(location));
		}
	}

	[Test]
	[TestCase(0, 1)]
	public void GetExpectedValue_ReturnsPositiveInfinity(double location, double scale)
	{
		var levy = new LevyDistribution(location, scale);

		var expectedValue = levy.Expected;

		Assert.That(expectedValue, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	[TestCase(0, 1)]
	public void GetVariance_ReturnsPositiveInfinity(double location, double scale)
	{
		var levy = new LevyDistribution(location, scale);

		var variance = levy.Variance;

		Assert.That(variance, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void GetMinValue_ReturnsLocation()
	{
		var location = 3.0;
		var levy = new LevyDistribution(location, 1);

		var minValue = levy.Minimum;

		Assert.That(minValue, Is.EqualTo(location));
	}

	[Test]
	public void GetMaxValue_ReturnsPositiveInfinity()
	{
		var levy = new LevyDistribution(0, 1);

		var maxValue = levy.Maximum;

		Assert.That(maxValue, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void StatisticalProperties_HeavyTailedDistribution()
	{
		var levy = new LevyDistribution(0, 1);
		var samples = new double[SampleSize];

		for (var i = 0; i < SampleSize; i++)
			samples[i] = levy.Distribute();

		var maxValue = samples.Max();
		Assert.That(maxValue, Is.GreaterThan(1000));

		var reasonableValues = samples.Where(x => x < 100).ToArray();
		Assert.That(reasonableValues, Has.Length.GreaterThan(SampleSize * 0.95));
	}

	[Test]
	public void ScaleParameter_AffectsSpread()
	{
		var levySmallScale = new LevyDistribution(0, 0.5);
		var levyLargeScale = new LevyDistribution(0, 2.0);
		var samplesSmall = new double[1000];
		var samplesLarge = new double[1000];

		for (var i = 0; i < 1000; i++)
		{
			samplesSmall[i] = levySmallScale.Distribute();
			samplesLarge[i] = levyLargeScale.Distribute();
		}

		var avgSmall = samplesSmall.Average();
		var avgLarge = samplesLarge.Average();

		Assert.That(avgLarge, Is.GreaterThan(avgSmall));
	}

	[Test]
	public void NegativeScale_ThrowsException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var levyDistribution = new LevyDistribution(0, -1);
		});
	}

	[Test]
	public void ZeroScale_ProducesConstantAtLocation()
	{
		var location = 5.0;
		var levy = new LevyDistribution(location, 0);

		for (var i = 0; i < 10; i++)
		{
			var result = levy.Distribute();
			Assert.That(result, Is.EqualTo(location));
		}
	}
}