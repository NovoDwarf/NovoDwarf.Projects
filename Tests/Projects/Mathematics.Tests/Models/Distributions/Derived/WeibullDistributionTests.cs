using Mathematics.Distributions.Univariate.Continuous.Semibounded;
using Mathematics.Functions;

namespace Mathematics.Tests.Models.Distributions.Derived;

[TestFixture]
public class WeibullDistributionTests
{
	private const double Tolerance = 0.001;
	
	[Test]
	public void Constructor_NegativeScale_ThrowsArgumentOutOfRangeException()
	{
		var negativeScale = -1.0;
		var shape = 1.0;

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			var weibullDistribution = new WeibullDistribution(negativeScale, shape);
		});
	}

	[Test]
	public void Constructor_NegativeShape_ThrowsArgumentOutOfRangeException()
	{
		var scale = 1.0;
		var negativeShape = -1.0;

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			var weibullDistribution = new WeibullDistribution(scale, negativeShape);
		});
	}
	
	[Test]
	public void Calculate_ReturnsNonNegativeValue()
	{
		var weibull = new WeibullDistribution(1.0, 1.0);

		var result = weibull.Distribute();

		Assert.That(result, Is.GreaterThanOrEqualTo(0));
	}

	[Test]
	public void Calculate_MultipleCallsReturnDifferentValues()
	{
		var weibull = new WeibullDistribution(2.0, 1.5);
		var results = new HashSet<double>();

		for (var i = 0; i < 100; i++) results.Add(weibull.Distribute());

		Assert.That(results, Has.Count.GreaterThan(1));
	}

	[Test]
	public void Calculate_WithZeroScale_ReturnsZero()
	{
		var weibull = new WeibullDistribution(0.0, 1.0);

		var result = weibull.Distribute();

		Assert.That(result, Is.Zero);
	}

	[Test]
	public void Calculate_WithShapeOne_ReturnsExponentialDistribution()
	{
		var weibull = new WeibullDistribution(2.0, 1.0);

		var result = weibull.Distribute();

		Assert.That(result, Is.GreaterThanOrEqualTo(0));
	}

	[Test]
	public void GetExpectedValue_CalculatesCorrectly()
	{
		var scale = 2.0;
		var shape = 1.5;
		var weibull = new WeibullDistribution(scale, shape);
		var expected = scale * GammaFunction.Calculate(1.0 + 1.0 / shape);

		var result = weibull.Expected;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_WithShapeOne_ReturnsScale()
	{
		var scale = 3.0;
		var weibull = new WeibullDistribution(scale, 1.0);
		var expected = scale;

		var result = weibull.Expected;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_WithZeroScale_ReturnsZero()
	{
		var weibull = new WeibullDistribution(0.0, 2.0);

		var result = weibull.Expected;

		Assert.That(result, Is.Zero);
	}

	[Test]
	public void GetVariance_CalculatesCorrectly()
	{
		var scale = 2.0;
		var shape = 1.5;

		var weibull = new WeibullDistribution(scale, shape);
		var gamma1 = GammaFunction.Calculate(1.0 + 1.0 / shape);
		var gamma2 = GammaFunction.Calculate(1.0 + 2.0 / shape);
		var expected = scale * scale * (gamma2 - gamma1 * gamma1);

		var result = weibull.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_WithShapeOne_ReturnsScaleSquared()
	{
		var scale = 3.0;
		var weibull = new WeibullDistribution(scale, 1.0);
		var expected = scale * scale;

		var result = weibull.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_WithZeroScale_ReturnsZero()
	{
		var weibull = new WeibullDistribution(0.0, 2.0);

		var result = weibull.Variance;

		Assert.That(result, Is.Zero);
	}

	[Test]
	[TestCase(1.0, 0.5)]
	[TestCase(2.0, 1.0)]
	[TestCase(3.0, 2.0)]
	[TestCase(0.5, 3.0)]
	public void GetVariance_IsAlwaysNonNegative(double scale, double shape)
	{
		var weibull = new WeibullDistribution(scale, shape);

		var variance = weibull.Variance;

		Assert.That(variance, Is.GreaterThanOrEqualTo(0),
			$"Variance should be non-negative for scale={scale}, shape={shape}");
	}

	[Test]
	public void GetMinValue_AlwaysReturnsZero()
	{
		var weibull = new WeibullDistribution(1.0, 1.0);

		var result = weibull.Minimum;

		Assert.That(result, Is.Zero);
	}

	[Test]
	public void GetMaxValue_AlwaysReturnsPositiveInfinity()
	{
		var weibull = new WeibullDistribution(1.0, 1.0);

		var result = weibull.Maximum;

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_WithLargeShape_ReturnsValuesCloseToScale()
	{
		var scale = 2.0;
		var largeShape = 100.0;
		var weibull = new WeibullDistribution(scale, largeShape);

		for (var i = 0; i < 50; i++)
		{
			var result = weibull.Distribute();
			Assert.That(result, Is.GreaterThanOrEqualTo(0));
			Assert.That(result, Is.LessThanOrEqualTo(scale * 2));
		}
	}

	[Test]
	public void Calculate_WithSmallShape_ReturnsWideRangeOfValues()
	{
		var weibull = new WeibullDistribution(1.0, 0.5);
		var results = new List<double>();

		for (var i = 0; i < 100; i++) results.Add(weibull.Distribute());

		results.Sort();
		var range = results[^1] - results[0];
		Assert.That(range, Is.GreaterThan(1.0));
	}

	[Test]
	[TestCase(0.5, 0.8)]
	[TestCase(1.0, 1.0)]
	[TestCase(2.0, 1.5)]
	[TestCase(5.0, 3.0)]
	public void ExpectedValue_WithDifferentParameters_ReturnsPositiveValues(double scale, double shape)
	{
		var weibull = new WeibullDistribution(scale, shape);

		var expectedValue = weibull.Expected;

		Assert.That(expectedValue, Is.GreaterThan(0),
			$"Expected value should be positive for scale={scale}, shape={shape}");
	}
}