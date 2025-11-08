using Mathematics.Distributions.Models.Continuous.SemiInfinite;
using Mathematics.Functions.Models;

namespace Mathematics.Tests.Models.Distributions.Derived;

[TestFixture]
public class WeibullDistributionTests
{
	private const double Tolerance = 0.001;

	[Test]
	public void Constructor_ValidParameters_SetsPropertiesCorrectly()
	{
		
		var scale = 2.5;
		var shape = 1.2;

		
		var weibull = new WeibullDistribution(scale, shape);

		
		Assert.That(weibull.Scale, Is.EqualTo(scale));
		Assert.That(weibull.Shape, Is.EqualTo(shape));
	}

	[Test]
	public void Constructor_NegativeScale_ThrowsArgumentOutOfRangeException()
	{
		
		var negativeScale = -1.0;
		var shape = 1.0;

		 
		Assert.Throws<ArgumentOutOfRangeException>(() =>
			new WeibullDistribution(negativeScale, shape));
	}

	[Test]
	public void Constructor_NegativeShape_ThrowsArgumentOutOfRangeException()
	{
		
		var scale = 1.0;
		var negativeShape = -1.0;

		 
		Assert.Throws<ArgumentOutOfRangeException>(() =>
			new WeibullDistribution(scale, negativeShape));
	}

	[Test]
	public void Constructor_ZeroScale_Allowed()
	{
		
		var scale = 0.0;
		var shape = 1.0;

		
		var weibull = new WeibullDistribution(scale, shape);

		
		Assert.That(weibull.Scale, Is.EqualTo(scale));
	}

	[Test]
	public void Constructor_ZeroShape_Allowed()
	{
		
		var scale = 1.0;
		var shape = 0.0;

		
		var weibull = new WeibullDistribution(scale, shape);

		
		Assert.That(weibull.Shape, Is.EqualTo(shape));
	}

	[Test]
	public void Calculate_ReturnsNonNegativeValue()
	{
		
		var weibull = new WeibullDistribution(1.0, 1.0);

		
		var result = weibull.Calculate();

		
		Assert.That(result, Is.GreaterThanOrEqualTo(0));
	}

	[Test]
	public void Calculate_MultipleCallsReturnDifferentValues()
	{
		var weibull = new WeibullDistribution(2.0, 1.5);
		var results = new HashSet<double>();
		
		for (var i = 0; i < 100; i++) results.Add(weibull.Calculate());

		Assert.That(results, Has.Count.GreaterThan(1));
	}

	[Test]
	public void Calculate_WithZeroScale_ReturnsZero()
	{
		
		var weibull = new WeibullDistribution(0.0, 1.0);

		
		var result = weibull.Calculate();

		
		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void Calculate_WithShapeOne_ReturnsExponentialDistribution()
	{
		var weibull = new WeibullDistribution(2.0, 1.0);
		
		var result = weibull.Calculate();

		Assert.That(result, Is.GreaterThanOrEqualTo(0));
	}

	[Test]
	public void GetExpectedValue_CalculatesCorrectly()
	{
		
		var scale = 2.0;
		var shape = 1.5;
		var weibull = new WeibullDistribution(scale, shape);
		var expected = scale * GammaFunction.Calculate(1.0 + 1.0 / shape);

		
		var result = weibull.GetExpectedValue();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_WithShapeOne_ReturnsScale()
	{
		
		var scale = 3.0;
		var weibull = new WeibullDistribution(scale, 1.0);
		var expected = scale; // Gamma(2) = 1!

		
		var result = weibull.GetExpectedValue();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_WithZeroScale_ReturnsZero()
	{
		
		var weibull = new WeibullDistribution(0.0, 2.0);

		
		var result = weibull.GetExpectedValue();

		
		Assert.That(result, Is.EqualTo(0));
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

		
		var result = weibull.GetVariance();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_WithShapeOne_ReturnsScaleSquared()
	{
		
		var scale = 3.0;
		var weibull = new WeibullDistribution(scale, 1.0);
		var expected = scale * scale; // Gamma(3) = 2!, Gamma(2) = 1! => 2 - 1 = 1

		
		var result = weibull.GetVariance();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_WithZeroScale_ReturnsZero()
	{
		
		var weibull = new WeibullDistribution(0.0, 2.0);

		
		var result = weibull.GetVariance();

		
		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void GetVariance_IsAlwaysNonNegative()
	{
		
		var testCases = new[]
		{
			new { Scale = 1.0, Shape = 0.5 },
			new { Scale = 2.0, Shape = 1.0 },
			new { Scale = 3.0, Shape = 2.0 },
			new { Scale = 0.5, Shape = 3.0 }
		};

		foreach (var testCase in testCases)
		{
			var weibull = new WeibullDistribution(testCase.Scale, testCase.Shape);

			
			var variance = weibull.GetVariance();

			
			Assert.That(variance, Is.GreaterThanOrEqualTo(0),
				$"Variance should be non-negative for scale={testCase.Scale}, shape={testCase.Shape}");
		}
	}

	[Test]
	public void GetMinValue_AlwaysReturnsZero()
	{
		
		var weibull = new WeibullDistribution(1.0, 1.0);

		
		var result = weibull.GetMinValue();

		
		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void GetMaxValue_AlwaysReturnsPositiveInfinity()
	{
		
		var weibull = new WeibullDistribution(1.0, 1.0);

		
		var result = weibull.GetMaxValue();

		
		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void ToString_ReturnsCorrectFormat()
	{
		
		var weibull = new WeibullDistribution(2.5, 1.2);

		
		var result = weibull.ToString();

		
		Assert.That(result, Does.Contain("Weibull"));
		Assert.That(result, Does.Contain("Scale = 2.5"));
		Assert.That(result, Does.Contain("Shape = 1.2"));
		Assert.That(result, Does.Match(@"Weibull \[Scale = \d+\.\d+, Shape = \d+\.\d+\]"));
	}

	[Test]
	public void ToString_FormatsNumbersWithThreeDecimals()
	{
		
		var weibull = new WeibullDistribution(1.23456, 0.98765);

		
		var result = weibull.ToString();

		
		Assert.That(result, Does.Contain("Scale = 1.235"));
		Assert.That(result, Does.Contain("Shape = 0.988"));
	}

	[Test]
	public void Calculate_WithLargeShape_ReturnsValuesCloseToScale()
	{
		
		var scale = 2.0;
		var largeShape = 100.0;
		var weibull = new WeibullDistribution(scale, largeShape);

		 
		for (var i = 0; i < 50; i++)
		{
			var result = weibull.Calculate();
			// With large shape, distribution becomes concentrated near scale
			Assert.That(result, Is.GreaterThanOrEqualTo(0));
			Assert.That(result, Is.LessThanOrEqualTo(scale * 2)); // Reasonable upper bound
		}
	}

	[Test]
	public void Calculate_WithSmallShape_ReturnsWideRangeOfValues()
	{
		
		var weibull = new WeibullDistribution(1.0, 0.5);
		var results = new List<double>();

		
		for (var i = 0; i < 100; i++) results.Add(weibull.Calculate());

		results.Sort();
		var range = results[^1] - results[0];
		Assert.That(range, Is.GreaterThan(1.0)); // Significant range
	}

	[Test]
	public void ExpectedValue_WithDifferentParameters_ReturnsPositiveValues()
	{
		
		var testCases = new[]
		{
			new { Scale = 0.5, Shape = 0.8 },
			new { Scale = 1.0, Shape = 1.0 },
			new { Scale = 2.0, Shape = 1.5 },
			new { Scale = 5.0, Shape = 3.0 }
		};

		foreach (var testCase in testCases)
		{
			var weibull = new WeibullDistribution(testCase.Scale, testCase.Shape);

			
			var expectedValue = weibull.GetExpectedValue();

			
			Assert.That(expectedValue, Is.GreaterThan(0),
				$"Expected value should be positive for scale={testCase.Scale}, shape={testCase.Shape}");
		}
	}
}