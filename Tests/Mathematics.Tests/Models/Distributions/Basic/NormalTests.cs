using Mathematics.Extensions;
using Mathematics.Models.Distributions.Basic;
using Mathematics.Tests.Services;

namespace Mathematics.Tests.Models.Distributions.Basic;

[TestFixture]
public class NormalTests
{
    private const double Tolerance = 1e-10;
    private const int SampleSize = 10000;

    [Test]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
    {
        const double mean = 5.0;
        const double stdDev = 2.0;
        
        var distribution = new Normal(mean, stdDev);
        
        Assert.Multiple(() =>
        {
            Assert.That(distribution.Mean, Is.EqualTo(mean));
            Assert.That(distribution.StandardDeviation, Is.EqualTo(stdDev));
        });
    }

    [Test]
    public void Constructor_WithZeroStandardDeviation_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Normal(0, 0));
    }

    [Test]
    public void Constructor_WithNegativeStandardDeviation_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Normal(0, -1.5));
    }

    [Test]
    public void Calculate_StandardNormal_ProducesZeroMeanAndUnitVariance()
    {
        var distribution = new Normal(0, 1);
        
        var samples = TestsUtils.GenerateSamples<Normal>(distribution, SampleSize);
        var sampleMean = samples.Average();
        var sampleVariance = samples.Variance();
        
        Assert.Multiple(() =>
        {
            Assert.That(sampleMean, Is.EqualTo(0).Within(0.1));
            Assert.That(sampleVariance, Is.EqualTo(1).Within(0.1));
        });
    }

    [Test]
    public void Calculate_WithDifferentMeans_ShiftsDistribution()
    {
        var means = new[] { -2.0, 0.0, 5.0, 10.0 };
        const double stdDev = 1.0;
        
        foreach (var mean in means)
        {
            var distribution = new Normal(mean, stdDev);

            var samples = TestsUtils.GenerateSamples<Normal>(distribution, SampleSize);
            var sampleMean = samples.Average();
            
            Assert.That(sampleMean, Is.EqualTo(mean).Within(0.1), $"Failed for mean = {mean}");
        }
    }

    [Test]
    public void Calculate_WithDifferentStandardDeviations_ScalesDistribution()
    {
        const double mean = 0.0;
        var stdDevs = new[] { 0.5, 1.0, 2.0, 5.0 };
        
        foreach (var stdDev in stdDevs)
        {
            var distribution = new Normal(mean, stdDev);

            var samples = TestsUtils.GenerateSamples<Normal>(distribution, SampleSize);
            var sampleVariance = samples.Variance();
            var expectedVariance = stdDev * stdDev;
            
            // Assert
            Assert.That(sampleVariance, Is.EqualTo(expectedVariance).Within(0.1), $"Failed for stdDev = {stdDev}");
        }
    }

    [Test]
    public void GetExpectedValue_Always_ReturnsMean()
    {
        // Arrange
        var testCases = new[]
        {
            (mean: -5.0, stdDev: 1.0),
            (mean: 0.0, stdDev: 2.0),
            (mean: 10.0, stdDev: 0.5),
            (mean: 100.0, stdDev: 10.0)
        };
        
        foreach (var (mean, stdDev) in testCases)
        {
            var distribution = new Normal(mean, stdDev);
            
            // Act
            var result = distribution.GetExpectedValue();
            
            // Assert
            Assert.That(result, Is.EqualTo(mean).Within(Tolerance));
        }
    }

    [Test]
    public void GetVariance_Always_ReturnsSquaredStandardDeviation()
    {
        // Arrange
        var testCases = new[]
        {
            (mean: 0.0, stdDev: 1.0, expected: 1.0),
            (mean: 0.0, stdDev: 2.0, expected: 4.0),
            (mean: 5.0, stdDev: 0.5, expected: 0.25),
            (mean: -2.0, stdDev: 3.0, expected: 9.0)
        };
        
        foreach (var (mean, stdDev, expected) in testCases)
        {
            var distribution = new Normal(mean, stdDev);
            
            // Act
            var result = distribution.GetVariance();
            
            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }
    }

    [Test]
    public void GetMinValue_Always_ReturnsNegativeInfinity()
    {
        // Arrange
        var distribution = new Normal(0, 1);
        
        // Act
        var result = distribution.GetMinValue();
        
        // Assert
        Assert.That(result, Is.EqualTo(double.NegativeInfinity));
    }

    [Test]
    public void GetMaxValue_Always_ReturnsPositiveInfinity()
    {
        // Arrange
        var distribution = new Normal(0, 1);
        
        // Act
        var result = distribution.GetMaxValue();
        
        // Assert
        Assert.That(result, Is.EqualTo(double.PositiveInfinity));
    }
    
    [Test]
    public void Calculate_DistributionShape_ApproximatelyNormal()
    {
        // Arrange
        var distribution = new Normal(0, 1);
        var samples = TestsUtils.GenerateSamples<Normal>(distribution, SampleSize);
        
        // Act
        var skewness = samples.Skewness();
        var kurtosis = samples.Kurtosis();
        
        // Assert - нормальное распределение должно быть симметричным
        Assert.That(skewness, Is.EqualTo(0).Within(0.1)); // Коэффициент асимметрии ≈ 0
        Assert.That(kurtosis, Is.EqualTo(0).Within(0.5)); // Эксцесс ≈ 0
    }

    [Test]
    public void Calculate_EmpiricalRule_68_95_99_7()
    {
        // Arrange
        var distribution = new Normal(0, 1);
        var samples = TestsUtils.GenerateSamples<Normal>(distribution, SampleSize);
        
        // Act
        var within1Std = samples.Count(x => Math.Abs(x) <= 1) / (double)SampleSize;
        var within2Std = samples.Count(x => Math.Abs(x) <= 2) / (double)SampleSize;
        var within3Std = samples.Count(x => Math.Abs(x) <= 3) / (double)SampleSize;
        
        // Assert - правило 68-95-99.7
        Assert.That(within1Std, Is.EqualTo(0.6827).Within(0.02)); // ~68%
        Assert.That(within2Std, Is.EqualTo(0.9545).Within(0.02)); // ~95%
        Assert.That(within3Std, Is.EqualTo(0.9973).Within(0.01)); // ~99.7%
    }

    [Test]
    public void Calculate_MultipleCalls_ProducesDifferentValues()
    {
        // Arrange
        var distribution = new Normal(0, 1);
        var results = new HashSet<double>();
        
        // Act
        for (var i = 0; i < 100; i++)
        {
            results.Add(distribution.Calculate());
        }
        
        // Assert - большинство значений должны быть уникальными
        Assert.That(results.Count, Is.GreaterThan(50));
    }

    [Test]
    public void Calculate_WithExtremeParameters_ProducesValidNumbers()
    {
        // Arrange
        var distribution = new Normal(1e6, 1e3);
        
        // Act
        var samples = TestsUtils.GenerateSamples<Normal>(distribution, 1000);
        
        // Assert - все значения должны быть конечными числами
        Assert.That(samples.All(x => !double.IsInfinity(x) && !double.IsNaN(x)), Is.True);
    }

    [Test]
    public void Calculate_SampleQuantiles_MatchNormalDistribution()
    {
        // Arrange
        var distribution = new Normal(0, 1);
        var samples = TestsUtils.GenerateSamples<Normal>(distribution, SampleSize);
        var sorted = samples.OrderBy(x => x).ToList();
        
        // Act & Assert - проверяем квантили
        var median = sorted.Median();
        var q1 = sorted[SampleSize / 4];
        var q3 = sorted[3 * SampleSize / 4];
        
        Assert.That(median, Is.EqualTo(0).Within(0.1));
        
        Assert.Multiple(() =>
        {
            Assert.That(q1, Is.EqualTo(-0.6745).Within(0.1));
            Assert.That(q3, Is.EqualTo(0.6745).Within(0.1));
        });
    }
}