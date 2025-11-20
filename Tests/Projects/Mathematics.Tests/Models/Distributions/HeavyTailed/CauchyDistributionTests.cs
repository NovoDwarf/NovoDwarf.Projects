using Mathematics.Core.Distributions.Models.Univariate.Continuous.RealLine;
using Mathematics.Core.Statistics.Extensions;

namespace Mathematics.Tests.Models.Distributions.HeavyTailed;

[TestFixture]
public class CauchyDistributionTests
{
	private const double Tolerance = 0.001;

	[Test]
	public void Calculate_ReturnsFiniteValue()
	{
		var cauchy = new CauchyDistribution(0, 1);

		var result = cauchy.Distribute();

		Assert.That(double.IsFinite(result), Is.True);
	}

	[Test]
	public void Calculate_MultipleCallsReturnDifferentValues()
	{
		var cauchy = new CauchyDistribution(0, 1);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++) results.Add(cauchy.Distribute());

		Assert.That(results, Has.Count.GreaterThan(1));
	}

	[Test]
	public void Calculate_WithZeroScale_ReturnsLocation()
	{
		var location = 5.0;
		var cauchy = new CauchyDistribution(location, 0);

		var result = cauchy.Distribute();

		Assert.That(result, Is.EqualTo(location).Within(Tolerance));
	}

	[Test]
	public void Calculate_WithNegativeScale_ReturnsValidValue()
	{
		var cauchy = new CauchyDistribution(0, -1.0);

		var result = cauchy.Distribute();

		Assert.That(double.IsFinite(result), Is.True);
	}

	[Test]
	[TestCase(0.0, 1000.0)]
	[TestCase(-1000.0, 0.001)]
	[TestCase(double.MaxValue / 2, 1.0)]
	[TestCase(double.MinValue / 2, 1.0)]
	public void Calculate_WithExtremeValues_ReturnsFiniteResults(double location, double scale)
	{
		var cauchy = new CauchyDistribution(location, scale);

		var result = cauchy.Distribute();

		Assert.That(double.IsFinite(result), Is.True,
			$"Should return finite value for location={location}, scale={scale}");
	}

	[Test]
	public void Calculate_ValuesFollowCauchyDistribution()
	{
		var cauchy = new CauchyDistribution(0, 1);
		var results = new List<double>();

		for (var i = 0; i < 1000; i++) results.Add(cauchy.Distribute());

		var min = results.Min();
		var max = results.Max();
		var range = max - min;

		Assert.That(range, Is.GreaterThan(10));
	}

	[Test]
	public void Calculate_WithDifferentLocations_ShiftsDistribution()
	{
		const double scale = 1.0;
		const double location1 = 0.0;
		const double location2 = 10.0;

		var cauchy1 = new CauchyDistribution(location1, scale);
		var cauchy2 = new CauchyDistribution(location2, scale);

		var results1 = new List<double>();
		var results2 = new List<double>();

		for (var i = 0; i < 500; i++)
		{
			results1.Add(cauchy1.Distribute());
			results2.Add(cauchy2.Distribute());
		}

		var mean1 = results1.Average();
		var mean2 = results2.Average();
		var shift = mean2 - mean1;

		Assert.That(Math.Abs(shift - 10.0), Is.LessThan(5.0));
	}

	[Test]
	public void Calculate_WithDifferentScales_ChangesSpread()
	{
		var location = 0.0;
		var scale1 = 0.5;
		var scale2 = 2.0;

		var cauchy1 = new CauchyDistribution(location, scale1);
		var cauchy2 = new CauchyDistribution(location, scale2);

		var results1 = new List<double>();
		var results2 = new List<double>();

		for (var i = 0; i < 500; i++)
		{
			results1.Add(cauchy1.Distribute());
			results2.Add(cauchy2.Distribute());
		}

		var variance1 = results1.Variance();
		var variance2 = results2.Variance();

		Assert.That(variance2, Is.GreaterThan(variance1));
	}

	[Test]
	public void GetExpectedValue_AlwaysReturnsNaN()
	{
		var cauchy = new CauchyDistribution(0, 1);

		var result = cauchy.Expected;

		Assert.That(double.IsNaN(result), Is.True);
	}

	[Test]
	public void GetVariance_AlwaysReturnsNaN()
	{
		var cauchy = new CauchyDistribution(0, 1);

		var result = cauchy.Variance;

		Assert.That(double.IsNaN(result), Is.True);
	}

	[Test]
	public void GetMinValue_AlwaysReturnsNegativeInfinity()
	{
		var cauchy = new CauchyDistribution(0, 1);

		var result = cauchy.Minimum;

		Assert.That(result, Is.EqualTo(double.NegativeInfinity));
	}

	[Test]
	public void GetMaxValue_AlwaysReturnsPositiveInfinity()
	{
		var cauchy = new CauchyDistribution(0, 1);

		var result = cauchy.Maximum;

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_ProducesHeavyTails()
	{
		var cauchy = new CauchyDistribution(0, 1);
		var extremeCount = 0;
		var totalSamples = 10000;

		for (var i = 0; i < totalSamples; i++)
		{
			var value = cauchy.Distribute();

			if (Math.Abs(value) > 10.0)
				extremeCount++;
		}

		var extremeProportion = (double)extremeCount / totalSamples;
		Assert.That(extremeProportion, Is.GreaterThan(0.01));
	}

	[Test]
	[TestCase(double.MaxValue, 1.0)]
	[TestCase(double.MinValue, 1.0)]
	[TestCase(double.PositiveInfinity, 1.0)]
	[TestCase(double.NegativeInfinity, 1.0)]
	public void Calculate_WithLocationAtExtremeValues_WorksCorrectly(double location, double scale)
	{
		var cauchy = new CauchyDistribution(location, scale);

		var result = cauchy.Distribute();

		Assert.That(double.IsFinite(result), Is.True, $"Should handle location={location}");
	}

	[Test]
	public void Calculate_WithVeryLargeScale_ProducesVerySpreadOutValues()
	{
		var largeScale = 1000.0;
		var cauchy = new CauchyDistribution(0, largeScale);
		var results = new List<double>();

		for (var i = 0; i < 100; i++) results.Add(cauchy.Distribute());

		var range = results.Max() - results.Min();
		Assert.That(range, Is.GreaterThan(1000));
	}

	[Test]
	public void Calculate_WithVerySmallScale_ProducesTightlyClusteredValues()
	{
		var smallScale = 0.001;
		var location = 5.0;
		var cauchy = new CauchyDistribution(location, smallScale);
		var results = new List<double>();

		for (var i = 0; i < 100; i++)
			results.Add(cauchy.Distribute());

		foreach (var result in results) Assert.That(Math.Abs(result - location), Is.LessThan(1.0));
	}
}