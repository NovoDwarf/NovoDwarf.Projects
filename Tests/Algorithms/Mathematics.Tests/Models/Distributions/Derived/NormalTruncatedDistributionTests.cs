using Mathematics.Distributions.Models.Continuous.SemiInfinite;

namespace Mathematics.Tests.Models.Distributions.Derived;

[TestFixture]
public class NormalTruncatedDistributionTests
{
	private const double Tolerance = 0.001;

	[Test]
	public void Constructor_SetsPropertiesCorrectly()
	{
		var mean = 2.5;
		var standardDeviation = 1.2;
		var min = 0.5;
		var max = 4.5;
		
		var truncatedNormal = new NormalTruncatedDistribution(mean, standardDeviation, min, max);
        
		using (Assert.EnterMultipleScope())
        {
            Assert.That(truncatedNormal.Mean, Is.EqualTo(mean));
            Assert.That(truncatedNormal.StandardDeviation, Is.EqualTo(standardDeviation));
            Assert.That(truncatedNormal.Min, Is.EqualTo(min));
            Assert.That(truncatedNormal.Max, Is.EqualTo(max));
        }
    }

	[Test]
	public void Calculate_ReturnsValueWithinBounds()
	{
		var min = 1.0;
		var max = 3.0;
		var truncatedNormal = new NormalTruncatedDistribution(2.0, 0.5, min, max);
		
		var result = truncatedNormal.Calculate();
		
		Assert.That(result, Is.GreaterThanOrEqualTo(min));
		Assert.That(result, Is.LessThanOrEqualTo(max));
	}

	[Test]
	public void Calculate_MultipleCallsReturnValuesWithinBounds()
	{
		var min = -1.0;
		var max = 1.0;
		var truncatedNormal = new NormalTruncatedDistribution(0.0, 0.3, min, max);
		
		for (var i = 0; i < 100; i++)
		{
			var result = truncatedNormal.Calculate();
			Assert.That(result, Is.GreaterThanOrEqualTo(min));
			Assert.That(result, Is.LessThanOrEqualTo(max));
		}
	}

	[Test]
	public void Calculate_WithTightBounds_StillReturnsValidValues()
	{
		var min = 1.9;
		var max = 2.1;
		var truncatedNormal = new NormalTruncatedDistribution(2.0, 0.5, min, max);
		
		for (var i = 0; i < 50; i++)
		{
			var result = truncatedNormal.Calculate();
			Assert.That(result, Is.GreaterThanOrEqualTo(min));
			Assert.That(result, Is.LessThanOrEqualTo(max));
		}
	}

	[Test]
	public void Calculate_WithWideBounds_ReturnsVariedValues()
	{
		var truncatedNormal = new NormalTruncatedDistribution(0.0, 1.0, -10.0, 10.0);
		var results = new HashSet<double>();
		
		for (var i = 0; i < 100; i++) results.Add(truncatedNormal.Calculate());

		Assert.That(results, Has.Count.GreaterThan(1));
	}

	[Test]
	public void GetExpectedValue_AlwaysReturnsNaN()
	{
		var truncatedNormal = new NormalTruncatedDistribution(0, 1, -1, 1);
		
		var result = truncatedNormal.GetExpectedValue();
		
		Assert.That(double.IsNaN(result), Is.True);
	}

	[Test]
	public void GetVariance_AlwaysReturnsNaN()
	{
		var truncatedNormal = new NormalTruncatedDistribution(0, 1, -1, 1);
		
		var result = truncatedNormal.GetVariance();
		
		Assert.That(double.IsNaN(result), Is.True);
	}

	[Test]
	public void GetMinValue_ReturnsConstructorMin()
	{
		var min = -2.5;
		var truncatedNormal = new NormalTruncatedDistribution(0, 1, min, 2.5);
		
		var result = truncatedNormal.GetMinValue();
		
		Assert.That(result, Is.EqualTo(min));
	}

	[Test]
	public void GetMaxValue_ReturnsConstructorMax()
	{
		const double max = 3.5;
		var truncatedNormal = new NormalTruncatedDistribution(0, 1, -3.5, max);
		
		var result = truncatedNormal.GetMaxValue();
		
		Assert.That(result, Is.EqualTo(max));
	}
	
	[Test]
	public void Constructor_WithMinGreaterThanMax_ThrowsException()
	{
		var min = 5.0;
		var max = 3.0;
		
		Assert.Throws<ArgumentException>(() =>
		{
			var normalTruncatedDistribution = new NormalTruncatedDistribution(0, 1, min, max);
		});
	}

	[Test]
	public void Constructor_WithNegativeStandardDeviation_ThrowsException()
	{
		var negativeStd = -1.0;
		
		Assert.Throws<ArgumentException>(() =>
		{
			var normalTruncatedDistribution = new NormalTruncatedDistribution(0, negativeStd, -1, 1);
		});
	}

	[Test]
	public void Calculate_WithMeanOutsideBounds_StillReturnsValuesWithinBounds()
	{
		var mean = 5.0; // Outside bounds
		var min = 1.0;
		var max = 3.0;
		var truncatedNormal = new NormalTruncatedDistribution(mean, 0.5, min, max);
		
		for (var i = 0; i < 50; i++)
		{
			var result = truncatedNormal.Calculate();
			Assert.That(result, Is.GreaterThanOrEqualTo(min));
			Assert.That(result, Is.LessThanOrEqualTo(max));
		}
	}

	[Test]
	public void Calculate_WithZeroStandardDeviation_ReturnsValueWithinBounds()
	{
		var mean = 2.0;
		var min = 1.0;
		var max = 3.0;
		var truncatedNormal = new NormalTruncatedDistribution(mean, 0, min, max);
		
		var result = truncatedNormal.Calculate();
		
		Assert.That(result, Is.EqualTo(mean).Within(Tolerance));
		Assert.That(result, Is.GreaterThanOrEqualTo(min));
		Assert.That(result, Is.LessThanOrEqualTo(max));
	}

	[Test]
	public void Calculate_WithMeanAtMinBound_ReturnsValidValues()
	{
		var mean = 0.0;
		var min = 0.0;
		var max = 2.0;
		var truncatedNormal = new NormalTruncatedDistribution(mean, 0.5, min, max);
		
		for (var i = 0; i < 50; i++)
		{
			var result = truncatedNormal.Calculate();
			Assert.That(result, Is.GreaterThanOrEqualTo(min));
			Assert.That(result, Is.LessThanOrEqualTo(max));
		}
	}

	[Test]
	public void Calculate_WithMeanAtMaxBound_ReturnsValidValues()
	{
		var mean = 2.0;
		var min = 0.0;
		var max = 2.0;
		var truncatedNormal = new NormalTruncatedDistribution(mean, 0.5, min, max);
		
		for (var i = 0; i < 50; i++)
		{
			var result = truncatedNormal.Calculate();
			Assert.That(result, Is.GreaterThanOrEqualTo(min));
			Assert.That(result, Is.LessThanOrEqualTo(max));
		}
	}
}