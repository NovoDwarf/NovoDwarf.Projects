using Mathematics.Distributions.Univariate.Continuous.Semibounded;

namespace Mathematics.Tests.Models.Distributions.Derived;

[TestFixture]
public class NormalLogDistributionTests
{
	private const double Tolerance = 0.001;

	[Test]
	public void Constructor_SetsPropertiesCorrectly()
	{
		var mean = 2.5;
		var standardDeviation = 1.2;

		var normalLog = new NormalLogDistribution(mean, standardDeviation);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(normalLog.Mean, Is.EqualTo(mean));
			Assert.That(normalLog.StandardDeviation, Is.EqualTo(standardDeviation));
		}
	}

	[Test]
	public void Calculate_ReturnsPositiveValue()
	{
		var normalLog = new NormalLogDistribution(0, 1);

		var result = normalLog.Distribute();

		Assert.That(result, Is.GreaterThan(0));
	}

	[Test]
	public void Calculate_MultipleCallsReturnDifferentValues()
	{
		var normalLog = new NormalLogDistribution(0, 1);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++) results.Add(normalLog.Distribute());

		Assert.That(results, Has.Count.GreaterThan(1));
	}

	[Test]
	public void GetExpectedValue_CalculatesCorrectly()
	{
		var mean = 1.0;
		var std = 0.5;
		var normalLog = new NormalLogDistribution(mean, std);
		var expected = Math.Exp(mean + std * std / 2.0);

		var result = normalLog.Expected;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_WithZeroMeanAndStd_ReturnsOne()
	{
		var normalLog = new NormalLogDistribution(0, 0);
		var expected = 1.0;

		var result = normalLog.Expected;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_CalculatesCorrectly()
	{
		var mean = 1.0;
		var std = 0.5;
		var normalLog = new NormalLogDistribution(mean, std);
		var expected = Math.Exp(2 * mean + std * std) * (Math.Exp(std * std) - 1);

		var result = normalLog.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_WithZeroMeanAndStd_ReturnsZero()
	{
		var normalLog = new NormalLogDistribution(0, 0);
		var expected = 0.0;

		var result = normalLog.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetMinValue_AlwaysReturnsZero()
	{
		var normalLog = new NormalLogDistribution(0, 1);

		var result = normalLog.Minimum;

		Assert.That(result, Is.Zero);
	}

	[Test]
	public void GetMaxValue_AlwaysReturnsPositiveInfinity()
	{
		var normalLog = new NormalLogDistribution(0, 1);

		var result = normalLog.Maximum;

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	[TestCase(-1.0, 0.5)]
	[TestCase(0.0, 1.0)]
	[TestCase(1.0, 2.0)]
	public void ExpectedValue_IsAlwaysPositive(double mean, double std)
	{
		var normalLog = new NormalLogDistribution(mean, std);

		var expectedValue = normalLog.Expected;

		Assert.That(expectedValue, Is.GreaterThan(0),
			$"Expected value should be positive for mean={mean}, std={std}");
	}

	[Test]
	[TestCase(-1.0, 0.5)]
	[TestCase(0.0, 1.0)]
	[TestCase(1.0, 2.0)]
	public void Variance_IsAlwaysNonNegative(double mean, double std)
	{
		var normalLog = new NormalLogDistribution(mean, std);

		var variance = normalLog.Variance;

		Assert.That(variance, Is.GreaterThanOrEqualTo(0),
			$"Variance should be non-negative for mean={mean}, std={std}");
	}
}