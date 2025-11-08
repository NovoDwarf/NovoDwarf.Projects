using Mathematics.Distributions.Models.Continuous.SemiInfinite;

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

		
		Assert.That(normalLog.Mean, Is.EqualTo(mean));
		Assert.That(normalLog.StandardDeviation, Is.EqualTo(standardDeviation));
	}

	[Test]
	public void Calculate_ReturnsPositiveValue()
	{
		
		var normalLog = new NormalLogDistribution(0, 1);

		
		var result = normalLog.Calculate();

		
		Assert.That(result, Is.GreaterThan(0));
	}

	[Test]
	public void Calculate_MultipleCallsReturnDifferentValues()
	{
		
		var normalLog = new NormalLogDistribution(0, 1);
		var results = new HashSet<double>();

		
		for (var i = 0; i < 100; i++) results.Add(normalLog.Calculate());

		Assert.That(results, Has.Count.GreaterThan(1));
	}

	[Test]
	public void GetExpectedValue_CalculatesCorrectly()
	{
		
		var mean = 1.0;
		var std = 0.5;
		var normalLog = new NormalLogDistribution(mean, std);
		var expected = Math.Exp(mean + std * std / 2.0);

		
		var result = normalLog.GetExpectedValue();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_WithZeroMeanAndStd_ReturnsOne()
	{
		
		var normalLog = new NormalLogDistribution(0, 0);
		var expected = 1.0;

		
		var result = normalLog.GetExpectedValue();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_CalculatesCorrectly()
	{
		
		var mean = 1.0;
		var std = 0.5;
		var normalLog = new NormalLogDistribution(mean, std);
		var expected = Math.Exp(2 * mean + std * std) * (Math.Exp(std * std) - 1);

		
		var result = normalLog.GetVariance();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_WithZeroMeanAndStd_ReturnsZero()
	{
		
		var normalLog = new NormalLogDistribution(0, 0);
		var expected = 0.0;

		
		var result = normalLog.GetVariance();

		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetMinValue_AlwaysReturnsZero()
	{
		
		var normalLog = new NormalLogDistribution(0, 1);

		
		var result = normalLog.GetMinValue();

		
		Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void GetMaxValue_AlwaysReturnsPositiveInfinity()
	{
		
		var normalLog = new NormalLogDistribution(0, 1);

		
		var result = normalLog.GetMaxValue();

		
		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void ToString_ReturnsCorrectFormat()
	{
		
		var normalLog = new NormalLogDistribution(2.5, 1.2);

		
		var result = normalLog.ToString();

		
		Assert.That(result, Does.Contain("LogNormal"));
		Assert.That(result, Does.Contain("Mean = 2.5"));
		Assert.That(result, Does.Contain("std=1.2"));
		Assert.That(result, Does.Match(@"LogNormal \[Mean = \d+\.\d+, std=\d+\.\d+\)"));
	}

	[Test]
	public void ToString_FormatsNumbersWithThreeDecimals()
	{
		
		var normalLog = new NormalLogDistribution(1.23456, 0.98765);

		
		var result = normalLog.ToString();

		
		Assert.That(result, Does.Contain("Mean = 1.235"));
		Assert.That(result, Does.Contain("std=0.988"));
	}

	// Тесты для проверки математических свойств логнормального распределения
	[Test]
	public void ExpectedValue_IsAlwaysPositive()
	{
		
		var testCases = new[]
		{
			new { Mean = -1.0, Std = 0.5 },
			new { Mean = 0.0, Std = 1.0 },
			new { Mean = 1.0, Std = 2.0 }
		};

		foreach (var testCase in testCases)
		{
			var normalLog = new NormalLogDistribution(testCase.Mean, testCase.Std);

			
			var expectedValue = normalLog.GetExpectedValue();

			
			Assert.That(expectedValue, Is.GreaterThan(0),
				$"Expected value should be positive for mean={testCase.Mean}, std={testCase.Std}");
		}
	}

	[Test]
	public void Variance_IsAlwaysNonNegative()
	{
		
		var testCases = new[]
		{
			new { Mean = -1.0, Std = 0.5 },
			new { Mean = 0.0, Std = 1.0 },
			new { Mean = 1.0, Std = 2.0 }
		};

		foreach (var testCase in testCases)
		{
			var normalLog = new NormalLogDistribution(testCase.Mean, testCase.Std);

			
			var variance = normalLog.GetVariance();

			
			Assert.That(variance, Is.GreaterThanOrEqualTo(0),
				$"Variance should be non-negative for mean={testCase.Mean}, std={testCase.Std}");
		}
	}
}