using Mathematics.Extensions;
using Mathematics.Models.Distributions.Composite;

namespace Mathematics.Tests.Models.Distributions.Composite;

[TestFixture]
public class GammaTests
{
    private const double Tolerance = 1e-10;
    private const double StatisticalTolerance = 0.1;

    [Test]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        const double shape = 2.0;
        const double scale = 1.5;

        var distribution = new Gamma(shape, scale);

        Assert.Multiple(() =>
        {
            Assert.That(distribution.Shape, Is.EqualTo(shape));
            Assert.That(distribution.Scale, Is.EqualTo(scale));
        });
    }

    [Test]
    public void Constructor_NegativeShape_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const double shape = -1.0;
        const double scale = 1.0;

        // Act & Assert
        Assert.That(() => new Gamma(shape, scale), 
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Constructor_NegativeScale_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var shape = 2.0;
        var scale = -1.0;

        // Act & Assert
        Assert.That(() => new Gamma(shape, scale), 
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Constructor_ZeroShape_Valid()
    {
        // Arrange
        const double shape = 0.0;
        const double scale = 1.0;

        // Act
        var distribution = new Gamma(shape, scale);

        Assert.Multiple(() =>
        {
            Assert.That(distribution.Shape, Is.EqualTo(shape));
            Assert.That(distribution.Scale, Is.EqualTo(scale));
        });
    }

    [Test]
    public void Constructor_ZeroScale_Valid()
    {
        // Arrange
        var shape = 2.0;
        var scale = 0.0;

        // Act
        var distribution = new Gamma(shape, scale);

        // Assert
        Assert.That(distribution.Shape, Is.EqualTo(shape));
        Assert.That(distribution.Scale, Is.EqualTo(scale));
    }

    [Test]
    public void GetExpectedValue_ValidParameters_ReturnsCorrectValue()
    {
        // Arrange
        var shape = 3.0;
        var scale = 2.0;
        var distribution = new Gamma(shape, scale);
        var expected = shape * scale;

        // Act
        var result = distribution.GetExpectedValue();

        // Assert
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test]
    public void GetExpectedValue_ZeroShape_ReturnsZero()
    {
        // Arrange
        var shape = 0.0;
        var scale = 2.0;
        var distribution = new Gamma(shape, scale);

        // Act
        var result = distribution.GetExpectedValue();

        // Assert
        Assert.That(result, Is.EqualTo(0).Within(Tolerance));
    }

    [Test]
    public void GetExpectedValue_ZeroScale_ReturnsZero()
    {
        // Arrange
        var shape = 3.0;
        var scale = 0.0;
        var distribution = new Gamma(shape, scale);

        // Act
        var result = distribution.GetExpectedValue();

        // Assert
        Assert.That(result, Is.EqualTo(0).Within(Tolerance));
    }

    [Test]
    public void GetVariance_ValidParameters_ReturnsCorrectValue()
    {
        // Arrange
        var shape = 3.0;
        var scale = 2.0;
        var distribution = new Gamma(shape, scale);
        var expected = shape * scale * scale;

        // Act
        var result = distribution.GetVariance();

        // Assert
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test]
    public void GetVariance_ZeroShape_ReturnsZero()
    {
        // Arrange
        var shape = 0.0;
        var scale = 2.0;
        var distribution = new Gamma(shape, scale);

        // Act
        var result = distribution.GetVariance();

        // Assert
        Assert.That(result, Is.EqualTo(0).Within(Tolerance));
    }

    [Test]
    public void GetVariance_ZeroScale_ReturnsZero()
    {
        // Arrange
        var shape = 3.0;
        var scale = 0.0;
        var distribution = new Gamma(shape, scale);

        // Act
        var result = distribution.GetVariance();

        // Assert
        Assert.That(result, Is.EqualTo(0).Within(Tolerance));
    }

    [Test]
    public void GetMinValue_Always_ReturnsZero()
    {
        // Arrange
        var distribution = new Gamma(2.0, 1.0);

        // Act
        var result = distribution.GetMinValue();

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void GetMaxValue_Always_ReturnsPositiveInfinity()
    {
        // Arrange
        var distribution = new Gamma(2.0, 1.0);

        // Act
        var result = distribution.GetMaxValue();

        // Assert
        Assert.That(result, Is.EqualTo(double.PositiveInfinity));
    }

    [Test]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var distribution = new Gamma(2.5, 1.75);

        // Act
        var result = distribution.ToString();

        // Assert
        Assert.That(result, Does.Contain("Gamma"));
        Assert.That(result, Does.Contain("Shape = 2.500"));
        Assert.That(result, Does.Contain("Scale = 1.750"));
    }

    [Test]
    public void Calculate_ShapeGreaterThanOne_ProducesValidValues()
    {
        // Arrange
        var distribution = new Gamma(2.5, 1.0);

        // Act & Assert
        for (var i = 0; i < 100; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
            Assert.That(value, Is.Not.EqualTo(double.NaN));
            Assert.That(value, Is.Not.EqualTo(double.PositiveInfinity));
        }
    }

    [Test]
    public void Calculate_ShapeLessThanOne_ProducesValidValues()
    {
        // Arrange
        var distribution = new Gamma(0.5, 1.0);

        // Act & Assert
        for (var i = 0; i < 100; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
            Assert.That(value, Is.Not.EqualTo(double.NaN));
            Assert.That(value, Is.Not.EqualTo(double.PositiveInfinity));
        }
    }

    [Test]
    public void Calculate_ShapeEqualOne_ProducesValidValues()
    {
        // Arrange
        var distribution = new Gamma(1.0, 1.0);

        // Act & Assert
        for (var i = 0; i < 100; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
            Assert.That(value, Is.Not.EqualTo(double.NaN));
            Assert.That(value, Is.Not.EqualTo(double.PositiveInfinity));
        }
    }

    [Test]
    public void Calculate_ZeroScale_ReturnsZero()
    {
        // Arrange
        var distribution = new Gamma(2.0, 0.0);

        // Act & Assert
        for (var i = 0; i < 10; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.EqualTo(0).Within(Tolerance));
        }
    }

    [Test]
    public void Calculate_ZeroShape_ReturnsZero()
    {
        // Arrange
        var distribution = new Gamma(0.0, 2.0);

        // Act & Assert
        for (var i = 0; i < 10; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.EqualTo(0).Within(Tolerance));
        }
    }

    [Test]
    public void Calculate_StatisticalProperties_ShapeGreaterThanOne()
    {
        // Arrange
        const double shape = 3.0;
        const double scale = 2.0;
        var distribution = new Gamma(shape, scale);
        var samples = new double[10000];
        var expectedMean = distribution.GetExpectedValue();
        var expectedVariance = distribution.GetVariance();

        // Act
        for (var i = 0; i < samples.Length; i++)
        {
            samples[i] = distribution.Calculate();
        }

        var actualMean = samples.Average();
        var actualVariance = samples.Variance();

        // Assert
        Assert.That(actualMean, Is.EqualTo(expectedMean).Within(StatisticalTolerance));
        Assert.That(actualVariance, Is.EqualTo(expectedVariance).Within(StatisticalTolerance));
    }

    [Test]
    public void Calculate_StatisticalProperties_ShapeLessThanOne()
    {
        // Arrange
        var shape = 0.5;
        var scale = 2.0;
        var distribution = new Gamma(shape, scale);
        var samples = new double[10000];
        var expectedMean = distribution.GetExpectedValue();
        var expectedVariance = distribution.GetVariance();

        // Act
        for (var i = 0; i < samples.Length; i++)
        {
            samples[i] = distribution.Calculate();
        }

        var actualMean = samples.Average();
        var actualVariance = samples.Variance();

        // Assert
        Assert.That(actualMean, Is.EqualTo(expectedMean).Within(StatisticalTolerance));
        Assert.That(actualVariance, Is.EqualTo(expectedVariance).Within(StatisticalTolerance));
    }

    [Test]
    public void Calculate_LargeShape_ProducesValidValues()
    {
        // Arrange
        var distribution = new Gamma(100.0, 1.0);

        // Act & Assert
        for (var i = 0; i < 50; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
            Assert.That(value, Is.Not.EqualTo(double.NaN));
            Assert.That(value, Is.InRange(50, 150));
        }
    }

    [Test]
    public void Calculate_SmallShape_ProducesValidValues()
    {
        // Arrange
        var distribution = new Gamma(0.1, 1.0);

        // Act & Assert
        for (var i = 0; i < 50; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
            Assert.That(value, Is.Not.EqualTo(double.NaN));
            Assert.That(value, Is.LessThan(1e6));
        }
    }

    [Test]
    public void Calculate_DifferentScales_ScalesCorrectly()
    {
        var distribution1 = new Gamma(2.0, 1.0);
        var distribution2 = new Gamma(2.0, 3.0);
        
        var samples1 = Enumerable.Range(0, 100).Select(_ => distribution1.Calculate()).ToArray();
        var samples2 = Enumerable.Range(0, 100).Select(_ => distribution2.Calculate()).ToArray();

        var mean1 = samples1.Average();
        var mean2 = samples2.Average();
        var ratio = mean2 / mean1;

        Assert.That(ratio, Is.EqualTo(3.0).Within(StatisticalTolerance));
    }
}
