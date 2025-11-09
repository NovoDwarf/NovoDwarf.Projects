using Mathematics.Distributions.Models.Continuous.SemiInfinite;

namespace Mathematics.Tests.Models.Distributions.Composite;

[TestFixture]
public class ExpoHyperDistributionTests
{
	private const double Tolerance = 1e-10;

	[Test]
	public void Constructor_ValidParameters_CreatesInstance()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [1.0, 2.0];
		
		var distribution = new ExpoHyperDistribution(probabilities, rates);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(distribution.Probabilities, Is.EqualTo(probabilities));
            Assert.That(distribution.Rates, Is.EqualTo(rates));
        }
    }

	[Test]
	public void Constructor_NullProbabilities_ThrowsArgumentNullException()
	{
		double[]? probabilities = null;
		double[] rates = [1.0, 2.0];
		
		Assert.That(() => new ExpoHyperDistribution(probabilities, rates),
			Throws.ArgumentNullException.With.Message.Contains("Probabilities and rates cannot be null"));
	}

	[Test]
	public void Constructor_NullRates_ThrowsArgumentNullException()
	{
		double[] probabilities = [0.3, 0.7];
		double[]? rates = null;
		
		Assert.That(() => new ExpoHyperDistribution(probabilities, rates),
			Throws.ArgumentNullException.With.Message.Contains("Probabilities and rates cannot be null"));
	}

	[Test]
	public void Constructor_DifferentLengths_ThrowsArgumentException()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [1.0];
		
		Assert.That(() => new ExpoHyperDistribution(probabilities, rates),
			Throws.ArgumentException.With.Message.Contains("Probabilities and rates must have same length"));
	}

	[Test]
	public void Constructor_EmptyArrays_ThrowsArgumentException()
	{
		double[] probabilities = [];
		double[] rates = [];
		
		Assert.That(() => new ExpoHyperDistribution(probabilities, rates),
			Throws.ArgumentException.With.Message.Contains("At least one component required"));
	}

	[Test]
	public void Constructor_NegativeProbabilities_ThrowsArgumentException()
	{
		double[] probabilities = [-0.1, 1.1];
		double[] rates = [1.0, 2.0];
		
		Assert.That(() => new ExpoHyperDistribution(probabilities, rates),
			Throws.ArgumentException.With.Message.Contains("Probabilities must be non-negative"));
	}

	[Test]
	public void Constructor_NonPositiveRates_ThrowsArgumentException()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [0.0, 2.0];
		
		Assert.That(() => new ExpoHyperDistribution(probabilities, rates),
			Throws.ArgumentException.With.Message.Contains("Rates must be positive"));
	}

	[Test]
	public void Constructor_ZeroSumProbabilities_ThrowsArgumentException()
	{
		double[] probabilities = [0.0, 0.0];
		double[] rates = [1.0, 2.0];
		
		Assert.That(() => new ExpoHyperDistribution(probabilities, rates),
			Throws.ArgumentException.With.Message.Contains("Sum of probabilities must be positive"));
	}

	[Test]
	public void Constructor_UnnormalizedProbabilities_NormalizesInternally()
	{
		double[] probabilities = [1.0, 2.0, 3.0];
		double[] rates = [1.0, 2.0, 3.0];

		var distribution = new ExpoHyperDistribution(probabilities, rates);

        using (Assert.EnterMultipleScope())
        {
			Assert.That(distribution.Probabilities, Is.EqualTo(probabilities));
			Assert.That(distribution.Rates, Is.EqualTo(rates));
		}
	}

	[Test]
	public void GetExpectedValue_SingleComponent_ReturnsCorrectValue()
	{
		double[] probabilities = [1.0];
		double[] rates = [2.0];
		
		var distribution = new ExpoHyperDistribution(probabilities, rates);
		var expected = 1.0 / 2.0;
		
		var result = distribution.GetExpectedValue();
		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_MultipleComponents_ReturnsCorrectValue()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [1.0, 2.0];
		
		var distribution = new ExpoHyperDistribution(probabilities, rates);
		var expected = 0.3 * (1.0 / 1.0) + 0.7 * (1.0 / 2.0);
		
		var result = distribution.GetExpectedValue();
		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetExpectedValue_UnnormalizedProbabilities_ReturnsCorrectValue()
	{
		double[] probabilities = [1.0, 2.0];
		double[] rates = [1.0, 2.0];
		
		var distribution = new ExpoHyperDistribution(probabilities, rates);
		var expected = 1.0 / 3.0 * (1.0 / 1.0) + 2.0 / 3.0 * (1.0 / 2.0);
		
		var result = distribution.GetExpectedValue();
		
		Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
	}

	[Test]
	public void GetVariance_SingleComponent_ReturnsCorrectValue()
	{
		double[] probabilities = [1.0];
		double[] rates = [2.0];
		
		var distribution = new ExpoHyperDistribution(probabilities, rates);
		var expectedVariance = 1.0 / (2.0 * 2.0);
		
		var result = distribution.GetVariance();
		
		Assert.That(result, Is.EqualTo(expectedVariance).Within(Tolerance));
	}

	[Test]
	public void GetVariance_MultipleComponents_ReturnsCorrectValue()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [1.0, 2.0];
		var distribution = new ExpoHyperDistribution(probabilities, rates);

		var eX = distribution.GetExpectedValue();
		var eX2 = 0.3 * (2.0 / (1.0 * 1.0)) + 0.7 * (2.0 / (2.0 * 2.0));
		var expectedVariance = eX2 - eX * eX;
		
		var result = distribution.GetVariance();
		
		Assert.That(result, Is.EqualTo(expectedVariance).Within(Tolerance));
	}

	[Test]
	public void GetMinValue_Always_ReturnsZero()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [1.0, 2.0];
		var distribution = new ExpoHyperDistribution(probabilities, rates);
		
		var result = distribution.GetMinValue();
		
		Assert.That(result, Is.Zero);
	}

	[Test]
	public void GetMaxValue_Always_ReturnsPositiveInfinity()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [1.0, 2.0];
		
		var distribution = new ExpoHyperDistribution(probabilities, rates);
		
		var result = distribution.GetMaxValue();
		
		Assert.That(result, Is.EqualTo(double.PositiveInfinity));
	}

	[Test]
	public void Calculate_ProducesValidValues()
	{
		double[] probabilities = [0.3, 0.7];
		double[] rates = [1.0, 2.0];
		
		var distribution = new ExpoHyperDistribution(probabilities, rates);

		for (var i = 0; i < 100; i++)
		{
			var value = distribution.Calculate();
			Assert.That(value, Is.GreaterThanOrEqualTo(0));
		}
	}
	
	[Test]
	public void Calculate_WithDifferentRandomValues_SelectsCorrectComponents()
	{
		double[] probabilities = [0.5, 0.5];
		double[] rates = [1.0, 10.0];
		var distribution = new ExpoHyperDistribution(probabilities, rates);

		var values = new double[1000];
		
		for (var i = 0; i < values.Length; i++)
		{
			values[i] = distribution.Calculate();
			Assert.That(values[i], Is.GreaterThanOrEqualTo(0));
		}

		var average = values.Average();
		var expectedMean = distribution.GetExpectedValue();
		Assert.That(average, Is.EqualTo(expectedMean).Within(0.5));
	}

	[Test]
	public void Distribution_ThreeComponents_CorrectProperties()
	{
		double[] probabilities = [0.2, 0.3, 0.5];
		double[] rates = [1.0, 2.0, 3.0];
		var distribution = new ExpoHyperDistribution(probabilities, rates);
		
		var mean = distribution.GetExpectedValue();
		var variance = distribution.GetVariance();
		var min = distribution.GetMinValue();
		var max = distribution.GetMaxValue();
		
		var expectedMean = 0.2 * 1.0 + 0.3 * 0.5 + 0.5 * (1.0 / 3.0);
        
		using (Assert.EnterMultipleScope())
        {
            Assert.That(mean, Is.EqualTo(expectedMean).Within(Tolerance));
            Assert.That(variance, Is.GreaterThan(0));
            Assert.That(min, Is.Zero);
            Assert.That(max, Is.EqualTo(double.PositiveInfinity));
        }
    }
}