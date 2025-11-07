using Mathematics.Models.Distributions.HeavyTailed;

namespace Mathematics.Tests.Models.Distributions.HeavyTailed;

[TestFixture]
public class CauchyTests
{
    private const double Tolerance = 0.001;

    [Test]
    public void Constructor_ValidParameters_SetsPropertiesCorrectly()
    {
        // Arrange
        double location = 2.5;
        double scale = 1.2;

        // Act
        var cauchy = new Cauchy(location, scale);

        // Assert
        Assert.That(cauchy.Location, Is.EqualTo(location));
        Assert.That(cauchy.Scale, Is.EqualTo(scale));
    }

    [Test]
    public void Constructor_ZeroScale_Allowed()
    {
        // Arrange
        double location = 2.5;
        double scale = 0.0;

        // Act
        var cauchy = new Cauchy(location, scale);

        // Assert
        Assert.That(cauchy.Scale, Is.EqualTo(0.0));
    }

    [Test]
    public void Constructor_NegativeScale_Allowed()
    {
        // Arrange
        double location = 2.5;
        double negativeScale = -1.0;

        // Act
        var cauchy = new Cauchy(location, negativeScale);

        // Assert
        Assert.That(cauchy.Scale, Is.EqualTo(negativeScale));
    }

    [Test]
    public void Calculate_ReturnsFiniteValue()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);

        // Act
        double result = cauchy.Calculate();

        // Assert
        Assert.That(double.IsFinite(result), Is.True);
    }

    [Test]
    public void Calculate_MultipleCallsReturnDifferentValues()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);
        var results = new HashSet<double>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            results.Add(cauchy.Calculate());
        }

        // Assert - should have some variation due to randomness
        Assert.That(results.Count, Is.GreaterThan(1));
    }

    [Test]
    public void Calculate_WithZeroScale_ReturnsLocation()
    {
        // Arrange
        double location = 5.0;
        var cauchy = new Cauchy(location, 0);

        // Act
        double result = cauchy.Calculate();

        // Assert
        Assert.That(result, Is.EqualTo(location).Within(Tolerance));
    }

    [Test]
    public void Calculate_WithNegativeScale_ReturnsValidValue()
    {
        // Arrange
        var cauchy = new Cauchy(0, -1.0);

        // Act
        double result = cauchy.Calculate();

        // Assert
        Assert.That(double.IsFinite(result), Is.True);
    }

    [Test]
    public void Calculate_WithExtremeValues_ReturnsFiniteResults()
    {
        // Arrange
        var testCases = new[]
        {
            new { Location = 0.0, Scale = 1000.0 },
            new { Location = -1000.0, Scale = 0.001 },
            new { Location = double.MaxValue / 2, Scale = 1.0 },
            new { Location = double.MinValue / 2, Scale = 1.0 }
        };

        foreach (var testCase in testCases)
        {
            var cauchy = new Cauchy(testCase.Location, testCase.Scale);

            // Act
            double result = cauchy.Calculate();

            // Assert
            Assert.That(double.IsFinite(result), Is.True,
                $"Should return finite value for location={testCase.Location}, scale={testCase.Scale}");
        }
    }

    [Test]
    public void Calculate_ValuesFollowCauchyDistribution()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);
        var results = new List<double>();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            results.Add(cauchy.Calculate());
        }

        // Assert - Cauchy distribution should produce both very small and very large values
        double min = results.Min();
        double max = results.Max();
        double range = max - min;

        Assert.That(range, Is.GreaterThan(10)); // Should have wide range
    }

    [Test]
    public void Calculate_WithDifferentLocations_ShiftsDistribution()
    {
        // Arrange
        double scale = 1.0;
        double location1 = 0.0;
        double location2 = 10.0;
        var cauchy1 = new Cauchy(location1, scale);
        var cauchy2 = new Cauchy(location2, scale);
        var results1 = new List<double>();
        var results2 = new List<double>();

        // Act
        for (int i = 0; i < 500; i++)
        {
            results1.Add(cauchy1.Calculate());
            results2.Add(cauchy2.Calculate());
        }

        // Assert - distributions should be shifted by approximately 10
        double mean1 = results1.Average();
        double mean2 = results2.Average();
        double shift = mean2 - mean1;

        Assert.That(Math.Abs(shift - 10.0), Is.LessThan(5.0)); // Allow for high variance
    }

    [Test]
    public void Calculate_WithDifferentScales_ChangesSpread()
    {
        // Arrange
        double location = 0.0;
        double scale1 = 0.5;
        double scale2 = 2.0;
        var cauchy1 = new Cauchy(location, scale1);
        var cauchy2 = new Cauchy(location, scale2);
        var results1 = new List<double>();
        var results2 = new List<double>();

        // Act
        for (int i = 0; i < 500; i++)
        {
            results1.Add(cauchy1.Calculate());
            results2.Add(cauchy2.Calculate());
        }

        // Assert - larger scale should produce more spread-out values
        double variance1 = CalculateVariance(results1);
        double variance2 = CalculateVariance(results2);

        Assert.That(variance2, Is.GreaterThan(variance1));
    }

    [Test]
    public void GetExpectedValue_AlwaysReturnsNaN()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);

        // Act
        double result = cauchy.GetExpectedValue();

        // Assert
        Assert.That(double.IsNaN(result), Is.True);
    }

    [Test]
    public void GetVariance_AlwaysReturnsNaN()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);

        // Act
        double result = cauchy.GetVariance();

        // Assert
        Assert.That(double.IsNaN(result), Is.True);
    }

    [Test]
    public void GetMinValue_AlwaysReturnsNegativeInfinity()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);

        // Act
        double result = cauchy.GetMinValue();

        // Assert
        Assert.That(result, Is.EqualTo(double.NegativeInfinity));
    }

    [Test]
    public void GetMaxValue_AlwaysReturnsPositiveInfinity()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);

        // Act
        double result = cauchy.GetMaxValue();

        // Assert
        Assert.That(result, Is.EqualTo(double.PositiveInfinity));
    }

    [Test]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var cauchy = new Cauchy(2.5, 1.2);

        // Act
        string result = cauchy.ToString();

        // Assert
        Assert.That(result, Does.Contain("Cauchy"));
        Assert.That(result, Does.Contain("Location = 2.5"));
        Assert.That(result, Does.Contain("Scale = 1.2"));
        Assert.That(result, Does.Match(@"Cauchy \[Location = \d+\.\d+, Scale = \d+\.\d+\]"));
    }

    [Test]
    public void ToString_FormatsNumbersWithThreeDecimals()
    {
        // Arrange
        var cauchy = new Cauchy(1.23456, 0.98765);

        // Act
        string result = cauchy.ToString();

        // Assert
        Assert.That(result, Does.Contain("Location = 1.235"));
        Assert.That(result, Does.Contain("Scale = 0.988"));
    }

    [Test]
    public void Calculate_ProducesHeavyTails()
    {
        // Arrange
        var cauchy = new Cauchy(0, 1);
        int extremeCount = 0;
        int totalSamples = 10000;

        // Act
        for (int i = 0; i < totalSamples; i++)
        {
            double value = cauchy.Calculate();
            if (Math.Abs(value) > 10.0) // Values beyond 10 standard deviations
            {
                extremeCount++;
            }
        }

        // Assert - Cauchy should have more extreme values than normal distribution
        double extremeProportion = (double)extremeCount / totalSamples;
        Assert.That(extremeProportion, Is.GreaterThan(0.01)); // At least 1% extreme values
    }

    [Test]
    public void Calculate_WithLocationAtExtremeValues_WorksCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            new { Location = double.MaxValue, Scale = 1.0 },
            new { Location = double.MinValue, Scale = 1.0 },
            new { Location = double.PositiveInfinity, Scale = 1.0 },
            new { Location = double.NegativeInfinity, Scale = 1.0 }
        };

        foreach (var testCase in testCases)
        {
            // Note: Some of these might not be valid in practice, but testing edge cases
            if (double.IsFinite(testCase.Location))
            {
                var cauchy = new Cauchy(testCase.Location, testCase.Scale);

                // Act
                double result = cauchy.Calculate();

                // Assert
                Assert.That(double.IsFinite(result), Is.True,
                    $"Should handle location={testCase.Location}");
            }
        }
    }

    [Test]
    public void Calculate_WithVeryLargeScale_ProducesVerySpreadOutValues()
    {
        // Arrange
        double largeScale = 1000.0;
        var cauchy = new Cauchy(0, largeScale);
        var results = new List<double>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            results.Add(cauchy.Calculate());
        }

        // Assert
        double range = results.Max() - results.Min();
        Assert.That(range, Is.GreaterThan(1000)); // Very wide range
    }

    [Test]
    public void Calculate_WithVerySmallScale_ProducesTightlyClusteredValues()
    {
        // Arrange
        double smallScale = 0.001;
        double location = 5.0;
        var cauchy = new Cauchy(location, smallScale);
        var results = new List<double>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            results.Add(cauchy.Calculate());
        }

        // Assert - values should be very close to location
        foreach (double result in results)
        {
            Assert.That(Math.Abs(result - location), Is.LessThan(1.0));
        }
    }

    private double CalculateVariance(List<double> values)
    {
        double mean = values.Average();
        double sumSquaredDifferences = values.Sum(x => (x - mean) * (x - mean));
        return sumSquaredDifferences / values.Count;
    }
}