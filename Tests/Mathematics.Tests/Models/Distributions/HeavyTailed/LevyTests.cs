using Mathematics.Models.Distributions.HeavyTailed;

namespace Mathematics.Tests.Models.Distributions.HeavyTailed;

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class LevyTests
{
    private const double Tolerance = 0.001;
    private const int SampleSize = 100000;
    
    [Test]
    [TestCase(2.5, 1.8)]
    public void Constructor_SetsPropertiesCorrectly(double location, double scale)
    {
        var levy = new Levy(location, scale);
       
        Assert.Multiple(() =>
        {
            Assert.That(levy.Location, Is.EqualTo(location));
            Assert.That(levy.Scale, Is.EqualTo(scale));
        });
    }

    [Test]
    [TestCase(0, 1)]
    public void Calculate_ReturnsValidValues(double location, double scale)
    {
        var levy = new Levy(location, scale);
        var results = new List<double>();
        
        for (var i = 0; i < 1000; i++)
        {
            var result = levy.Calculate();
            results.Add(result);
        }

        Assert.Multiple(() =>
        {
            Assert.That(results.All(double.IsFinite), Is.True);
            Assert.That(results.All(x => x >= levy.Location), Is.True);
        });
    }
    
    [Test]
    [TestCase(5.0, 2.0)]
    public void Calculate_RespectsLocationParameter(double location, double scale)
    {
        var levy = new Levy(location, scale);

        for (var i = 0; i < 100; i++)
        {
            var result = levy.Calculate();
            Assert.That(result, Is.GreaterThanOrEqualTo(location));
        }
    }
    
    [Test]
    [TestCase(0, 1)]
    public void GetExpectedValue_ReturnsPositiveInfinity(double location, double scale)
    {
        var levy = new Levy(location, scale);
        
        var expectedValue = levy.GetExpectedValue();
        
        Assert.That(expectedValue, Is.EqualTo(double.PositiveInfinity));
    }
    
    [Test]
    [TestCase(0, 1)]
    public void GetVariance_ReturnsPositiveInfinity(double location, double scale)
    {
        var levy = new Levy(location, scale);
        
        var variance = levy.GetVariance();
        
        Assert.That(variance, Is.EqualTo(double.PositiveInfinity));
    }
    
    [Test]
    public void GetMinValue_ReturnsLocation()
    {
        // Arrange
        var location = 3.0;
        var levy = new Levy(location, 1);
        
        // Act
        var minValue = levy.GetMinValue();
        
        // Assert
        Assert.That(minValue, Is.EqualTo(location));
    }
    
    [Test]
    public void GetMaxValue_ReturnsPositiveInfinity()
    {
        // Arrange
        var levy = new Levy(0, 1);
        
        // Act
        var maxValue = levy.GetMaxValue();
        
        // Assert
        Assert.That(maxValue, Is.EqualTo(double.PositiveInfinity));
    }
    
    [Test]
    public void StatisticalProperties_HeavyTailedDistribution()
    {
        // Arrange
        var levy = new Levy(0, 1);
        var samples = new double[SampleSize];
        
        // Act
        for (var i = 0; i < SampleSize; i++)
        {
            samples[i] = levy.Calculate();
        }
        
        // Assert - Levy distribution should have very high values occasionally
        // due to heavy tails
        var maxValue = samples.Max();
        Assert.That(maxValue, Is.GreaterThan(1000)); // Should see some very large values
        
        // Most values should be relatively small
        var reasonableValues = samples.Where(x => x < 100).ToArray();
        Assert.That(reasonableValues, Has.Length.GreaterThan(SampleSize * 0.95));
    }
    
    [Test]
    public void ScaleParameter_AffectsSpread()
    {
        // Arrange
        var levySmallScale = new Levy(0, 0.5);
        var levyLargeScale = new Levy(0, 2.0);
        var samplesSmall = new double[1000];
        var samplesLarge = new double[1000];
        
        // Act
        for (var i = 0; i < 1000; i++)
        {
            samplesSmall[i] = levySmallScale.Calculate();
            samplesLarge[i] = levyLargeScale.Calculate();
        }
        
        // Assert - Larger scale should generally produce larger values
        var avgSmall = samplesSmall.Average();
        var avgLarge = samplesLarge.Average();
        
        Assert.That(avgLarge, Is.GreaterThan(avgSmall));
    }
    
    [Test]
    public void NegativeScale_ThrowsException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Levy(0, -1));
    }
    
    [Test]
    public void ZeroScale_ProducesConstantAtLocation()
    {
        // Arrange
        var location = 5.0;
        var levy = new Levy(location, 0);
        
        // Act & Assert
        for (var i = 0; i < 10; i++)
        {
            var result = levy.Calculate();
            Assert.That(result, Is.EqualTo(location));
        }
    }
}