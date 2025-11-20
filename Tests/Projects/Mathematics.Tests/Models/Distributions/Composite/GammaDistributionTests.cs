using Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;
using Mathematics.Core.Statistics.Extensions;

namespace Mathematics.Tests.Models.Distributions.Composite;

[TestFixture]
public class GammaDistributionTests
{
	private const double Tolerance = 1e-10;
	private const double StatisticalTolerance = 0.1;
	
	[Test]
	public void Constructor_NegativeShape_ThrowsArgumentOutOfRangeException()
	{
		const double shape = -1.0;
		const double scale = 1.0;

		Assert.That(() => new GammaDistribution(shape, scale), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void Constructor_NegativeScale_ThrowsArgumentOutOfRangeException()
	{
		var shape = 2.0;
		var scale = -1.0;

		Assert.That(() => new GammaDistribution(shape, scale),
			Throws.InstanceOf<ArgumentOutOfRangeException>());
	}
	
	[Test]
	public void GetExpectedValue_ValidParameters_ReturnsCorrectValue()
	{
		var shape = 3.0;
		var scale = 2.0;
		
		var distribution = new GammaDistribution(shape, scale);
		var expected = shape * scale;

		var result = distribution.Expected;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_ZeroShape_ReturnsZero()
	{
		var shape = 0.0;
		var scale = 2.0;
		
		var distribution = new GammaDistribution(shape, scale);

		var result = distribution.Expected;

		Assert.That(result, Is.EqualTo(0).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_ZeroScale_ReturnsZero()
	{
		var shape = 3.0;
		var scale = 0.0;
		var distribution = new GammaDistribution(shape, scale);

		var result = distribution.Expected;

		Assert.That(result, Is.EqualTo(0).Within(Tolerance));
	}

	[Test]
	public void GetVariance_ValidParameters_ReturnsCorrectValue()
	{
		var shape = 3.0;
		var scale = 2.0;
		var distribution = new GammaDistribution(shape, scale);
		var expected = shape * scale * scale;

		var result = distribution.Variance;

		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_ZeroShape_ReturnsZero()
	{
		var shape = 0.0;
		var scale = 2.0;
		var distribution = new GammaDistribution(shape, scale);

		var result = distribution.Variance;


		Assert.That(result, Is.EqualTo(0).Within(Tolerance));
	}

	[Test]
	public void GetVariance_ZeroScale_ReturnsZero()
	{
		var shape = 3.0;
		var scale = 0.0;
		var distribution = new GammaDistribution(shape, scale);

		var result = distribution.Variance;

		Assert.That(result, Is.EqualTo(0).Within(Tolerance));
	}

	[Test]
	public void GetMinValue_Always_ReturnsZero()
	{
		var distribution = new GammaDistribution(2.0, 1.0);

		var result = distribution.Minimum;

		Assert.That(result, Is.Zero);
	}

	[Test]
	public void GetMaxValue_Always_ReturnsPositiveInfinity()
	{
		var distribution = new GammaDistribution(2.0, 1.0);

		var result = distribution.Maximum;

		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_ShapeGreaterThanOne_ProducesValidValues()
	{
		var distribution = new GammaDistribution(2.5, 1.0);

		for (var i = 0; i < 100; i++)
		{
			var value = distribution.Distribute();
			Assert.That(value, Is.GreaterThanOrEqualTo(0));
			Assert.That(value, Is.Not.EqualTo(double.NaN));
			Assert.That(value, Is.Not.EqualTo(double.PositiveInfinity));
		}
	}

	[Test]
	public void Calculate_ShapeLessThanOne_ProducesValidValues()
	{
		var distribution = new GammaDistribution(0.5, 1.0);

		for (var i = 0; i < 100; i++)
		{
			var value = distribution.Distribute();
			Assert.That(value, Is.GreaterThanOrEqualTo(0));
			Assert.That(value, Is.Not.EqualTo(double.NaN));
			Assert.That(value, Is.Not.EqualTo(double.PositiveInfinity));
		}
	}

	[Test]
	public void Calculate_ShapeEqualOne_ProducesValidValues()
	{
		var distribution = new GammaDistribution(1.0, 1.0);

		for (var i = 0; i < 100; i++)
		{
			var value = distribution.Distribute();
			Assert.That(value, Is.GreaterThanOrEqualTo(0));
			Assert.That(value, Is.Not.EqualTo(double.NaN));
			Assert.That(value, Is.Not.EqualTo(double.PositiveInfinity));
		}
	}

	[Test]
	public void Calculate_ZeroScale_ReturnsZero()
	{
		var distribution = new GammaDistribution(2.0, 0.0);

		for (var i = 0; i < 10; i++)
		{
			var value = distribution.Distribute();
			Assert.That(value, Is.EqualTo(0).Within(Tolerance));
		}
	}

	[Test]
	public void Calculate_ZeroShape_ReturnsZero()
	{
		var distribution = new GammaDistribution(0.0, 2.0);

		for (var i = 0; i < 10; i++)
		{
			var value = distribution.Distribute();
			Assert.That(value, Is.EqualTo(0).Within(Tolerance));
		}
	}

	[Test]
	public void Calculate_StatisticalProperties_ShapeGreaterThanOne()
	{
		const double shape = 3.0;
		const double scale = 2.0;
		var distribution = new GammaDistribution(shape, scale);
		var samples = new double[10000];
		var expectedMean = distribution.Expected;
		var expectedVariance = distribution.Variance;

		for (var i = 0; i < samples.Length; i++) samples[i] = distribution.Distribute();

		var actualMean = samples.Average();
		var actualVariance = samples.Variance();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(actualMean, Is.EqualTo(expectedMean).Within(StatisticalTolerance));
			Assert.That(actualVariance, Is.EqualTo(expectedVariance).Within(StatisticalTolerance));
		}
	}

	[Test]
	public void Calculate_StatisticalProperties_ShapeLessThanOne()
	{
		var shape = 0.5;
		var scale = 2.0;
		var distribution = new GammaDistribution(shape, scale);
		var samples = new double[10000];
		var expectedMean = distribution.Expected;
		var expectedVariance = distribution.Variance;

		for (var i = 0; i < samples.Length; i++) samples[i] = distribution.Distribute();

		var actualMean = samples.Average();
		var actualVariance = samples.Variance();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(actualMean, Is.EqualTo(expectedMean).Within(StatisticalTolerance));
			Assert.That(actualVariance, Is.EqualTo(expectedVariance).Within(StatisticalTolerance));
		}
	}

	[Test]
	public void Calculate_LargeShape_ProducesValidValues()
	{
		var distribution = new GammaDistribution(100.0, 1.0);

		for (var i = 0; i < 50; i++)
		{
			var value = distribution.Distribute();
			Assert.That(value, Is.GreaterThanOrEqualTo(0));
			Assert.That(value, Is.Not.EqualTo(double.NaN));
			Assert.That(value, Is.InRange(50, 150));
		}
	}

	[Test]
	public void Calculate_SmallShape_ProducesValidValues()
	{
		var distribution = new GammaDistribution(0.1, 1.0);

		for (var i = 0; i < 50; i++)
		{
			var value = distribution.Distribute();
			Assert.That(value, Is.GreaterThanOrEqualTo(0));
			Assert.That(value, Is.Not.EqualTo(double.NaN));
			Assert.That(value, Is.LessThan(1e6));
		}
	}

	[Test]
	public void Calculate_DifferentScales_ScalesCorrectly()
	{
		var distribution1 = new GammaDistribution(2.0, 1.0);
		var distribution2 = new GammaDistribution(2.0, 3.0);

		var samples1 = Enumerable.Range(0, 100).Select(_ => distribution1.Distribute()).ToArray();
		var samples2 = Enumerable.Range(0, 100).Select(_ => distribution2.Distribute()).ToArray();

		var mean1 = samples1.Average();
		var mean2 = samples2.Average();
		var ratio = mean2 / mean1;

		Assert.That(ratio, Is.EqualTo(3.0).Within(StatisticalTolerance));
	}
}