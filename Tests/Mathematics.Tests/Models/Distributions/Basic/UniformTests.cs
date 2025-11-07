using Mathematics.Extensions;
using Mathematics.Models.Distributions.Basic;
using Mathematics.Tests.Services;

namespace Mathematics.Tests.Models.Distributions.Basic;

[TestFixture]
public class UniformTests
{
    private const double Tolerance = 1e-10;
    private const int SampleSize = 10000;

    [Test]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
    {
        // Arrange
        const double min = 2.0;
        const double max = 5.0;
        
        // Act
        var distribution = new Uniform(min, max);
        
        Assert.Multiple(() =>
        {
            Assert.That(distribution.Min, Is.EqualTo(min));
            Assert.That(distribution.Max, Is.EqualTo(max));
        });
    }

    [Test]
    public void Constructor_WithMinEqualToMax_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Uniform(5.0, 5.0));
    }

    [Test]
    public void Constructor_WithMinGreaterThanMax_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => new Uniform(10.0, 5.0));
    }

    [Test]
    public void Calculate_StandardUniform_ProducesValuesInZeroToOneRange()
    {
        // Arrange
        var distribution = new Uniform(0, 1);
        
        // Act
        var samples = TestsUtils.GenerateSamples<Uniform>(distribution, SampleSize);
       
        Assert.Multiple(() =>
        {
            Assert.That(samples.All(x => x is >= 0 and < 1), Is.True);
            Assert.That(samples.Min(), Is.GreaterThanOrEqualTo(0));
            Assert.That(samples.Max(), Is.LessThan(1));
        });
    }

    [Test]
    public void Calculate_WithDifferentRanges_RespectsBounds()
    {
        // Arrange
        var testCases = new[]
        {
            (min: -5.0, max: 5.0),
            (min: 10.0, max: 20.0),
            (min: -100.0, max: -50.0),
            (min: 0.0, max: 0.001)
        };
        
        foreach (var (min, max) in testCases)
        {
            var distribution = new Uniform(min, max);
            
            // Act
            var samples = TestsUtils.GenerateSamples<Uniform>(distribution, 1000);
            
            // Assert
            Assert.That(samples.All(x => x >= min && x < max), Is.True,
                $"Failed for range [{min}, {max})");
        }
    }

    [Test]
    public void Calculate_ValuesAreUniformlyDistributed()
    {
        var distribution = new Uniform(0, 100);
        var samples = TestsUtils.GenerateSamples<Uniform>(distribution, SampleSize);
        var histogram = new int[10];

        foreach (var sample in samples)
        {
            var bin = (int)(sample / 10);
            if (bin >= 0 && bin < histogram.Length)
                histogram[bin]++;
        }
        
        var expectedCount = SampleSize / histogram.Length;
        
        foreach (var count in histogram)
        {
            Assert.That(count, Is.EqualTo(expectedCount).Within(expectedCount * 0.15)); // ±15%
        }
    }

    [Test]
    public void GetExpectedValue_WithVariousRanges_ReturnsCorrectMean()
    {
        // Arrange
        var testCases = new[]
        {
            (min: 0.0, max: 1.0, expected: 0.5),
            (min: -5.0, max: 5.0, expected: 0.0),
            (min: 10.0, max: 20.0, expected: 15.0),
            (min: -10.0, max: 0.0, expected: -5.0),
            (min: 2.5, max: 7.5, expected: 5.0)
        };
        
        foreach (var (min, max, expected) in testCases)
        {
            var distribution = new Uniform(min, max);
            
            // Act
            var result = distribution.GetExpectedValue();
            
            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance),
                $"Failed for range [{min}, {max})");
        }
    }

    [Test]
    public void GetVariance_WithVariousRanges_ReturnsCorrectValue()
    {
        // Arrange
        var testCases = new[]
        {
            (min: 0.0, max: 1.0, expected: 1.0 / 12.0),     // (1-0)²/12 = 1/12
            (min: 0.0, max: 2.0, expected: 4.0 / 12.0),     // (2-0)²/12 = 4/12
            (min: -1.0, max: 1.0, expected: 4.0 / 12.0),    // (1-(-1))²/12 = 4/12
            (min: 5.0, max: 10.0, expected: 25.0 / 12.0),   // (10-5)²/12 = 25/12
            (min: -3.0, max: 3.0, expected: 36.0 / 12.0)    // (3-(-3))²/12 = 36/12
        };
        
        foreach (var (min, max, expected) in testCases)
        {
            var distribution = new Uniform(min, max);
            
            // Act
            var result = distribution.GetVariance();
            
            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance),
                $"Failed for range [{min}, {max})");
        }
    }

    [Test]
    public void GetMinValue_Always_ReturnsMin()
    {
        // Arrange
        var distribution = new Uniform(2.5, 7.5);
        
        // Act
        var result = distribution.GetMinValue();
        
        // Assert
        Assert.That(result, Is.EqualTo(2.5));
    }

    [Test]
    public void GetMaxValue_Always_ReturnsMax()
    {
        // Arrange
        var distribution = new Uniform(2.5, 7.5);
        
        // Act
        var result = distribution.GetMaxValue();
        
        // Assert
        Assert.That(result, Is.EqualTo(7.5));
    }

    [Test]
    public void ToString_WithParameters_ReturnsFormattedString()
    {
        // Arrange
        var distribution = new Uniform(2.5, 7.5);
        
        // Act
        var result = distribution.ToString();
        
        // Assert
        Assert.That(result, Is.EqualTo("Uniform [Min = 2.500, Max = 7.500)"));
    }

    [Test]
    public void Calculate_SampleMeanAndVariance_ApproximateTheoretical()
    {
        const double min = 2.0;
        const double max = 8.0;
        var distribution = new Uniform(min, max);
        
        var samples = TestsUtils.GenerateSamples<Uniform>(distribution, SampleSize);
        var sampleMean = samples.Average();
        var sampleVariance = samples.Variance();
        
        const double expectedMean = (min + max) / 2.0;
        var expectedVariance = Math.Pow(max - min, 2) / 12.0;
        
        Assert.Multiple(() =>
        {
            Assert.That(sampleMean, Is.EqualTo(expectedMean).Within(0.05));
            Assert.That(sampleVariance, Is.EqualTo(expectedVariance).Within(0.05));
        });
    }

    [Test]
    public void Calculate_NoValuesOutsideRange()
    {
        var distribution = new Uniform(-10, 10);
        
        var samples = TestsUtils.GenerateSamples<Uniform>(distribution, SampleSize);
        
        Assert.Multiple(() =>
        {
            Assert.That(samples.Any(x => x < -10), Is.False);
            Assert.That(samples.Any(x => x >= 10), Is.False);
        });
    }

    [Test]
    public void Calculate_DistributionIsFlat_NoSkewness()
    {
        // Arrange
        var distribution = new Uniform(0, 1);
        var samples = TestsUtils.GenerateSamples<Uniform>(distribution, SampleSize);
        
        // Act
        var skewness = samples.Skewness();
        
        // Assert - равномерное распределение должно быть симметричным
        Assert.That(skewness, Is.EqualTo(0).Within(0.1));
    }

    [Test]
    public void Calculate_WithVerySmallRange_WorksCorrectly()
    {
        // Arrange
        var distribution = new Uniform(0.499, 0.501);
        
        // Act
        var samples = TestsUtils.GenerateSamples<Uniform>(distribution, 1000);
        
        Assert.Multiple(() =>
        {
            Assert.That(samples.All(x => x is >= 0.499 and < 0.501), Is.True);
            Assert.That(samples.Min(), Is.GreaterThanOrEqualTo(0.499));
            Assert.That(samples.Max(), Is.LessThan(0.501));
        });
    }

    [Test]
    public void Calculate_WithNegativeRange_WorksCorrectly()
    {
        // Arrange
        var distribution = new Uniform(-100, -50);
        
        // Act
        var samples = TestsUtils.GenerateSamples<Uniform>(distribution, 1000);
       
        Assert.Multiple(() =>
        {
            Assert.That(samples.All(x => x is >= -100 and < -50), Is.True);
            Assert.That(samples.Average(), Is.EqualTo(-75).Within(1.0));
        });
    }

    [Test]
    public void Calculate_MultipleCalls_ProducesDifferentValues()
    {
        // Arrange
        var distribution = new Uniform(0, 100);
        var results = new HashSet<double>();
        
        // Act
        for (var i = 0; i < 100; i++)
        {
            results.Add(distribution.Calculate());
        }
        
        // Assert - большинство значений должны быть уникальными
        Assert.That(results, Has.Count.GreaterThan(50));
    }

    [Test]
    public void Calculate_EdgeCaseValues_ApproachBoundsButNotEqual()
    {
        // Arrange
        var distribution = new Uniform(0, 1);
        const int attempts = 10000;
        var gotVeryCloseToMax = false;
        
        // Act
        for (var i = 0; i < attempts; i++)
        {
            var value = distribution.Calculate();
            
            if (value > 0.999)
                gotVeryCloseToMax = true;
            
            Assert.That(value, Is.LessThan(1.0));
        }
        
        Assert.That(gotVeryCloseToMax, Is.True);
    }
}