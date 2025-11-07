using Mathematics.Models.Distributions.Derived;

namespace Mathematics.Tests.Models.Distributions.Derived;

using NUnit.Framework;
using System;
using System.Collections.Generic;

    [TestFixture]
    public class NormalTruncatedTests
    {
        private const double Tolerance = 0.001;

        [Test]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            double mean = 2.5;
            double standardDeviation = 1.2;
            double min = 0.5;
            double max = 4.5;

            // Act
            var truncatedNormal = new NormalTruncated(mean, standardDeviation, min, max);

            // Assert
            Assert.That(truncatedNormal.Mean, Is.EqualTo(mean));
            Assert.That(truncatedNormal.StandardDeviation, Is.EqualTo(standardDeviation));
            Assert.That(truncatedNormal.Min, Is.EqualTo(min));
            Assert.That(truncatedNormal.Max, Is.EqualTo(max));
        }

        [Test]
        public void Calculate_ReturnsValueWithinBounds()
        {
            // Arrange
            double min = 1.0;
            double max = 3.0;
            var truncatedNormal = new NormalTruncated(2.0, 0.5, min, max);

            // Act
            double result = truncatedNormal.Calculate();

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(min));
            Assert.That(result, Is.LessThanOrEqualTo(max));
        }

        [Test]
        public void Calculate_MultipleCallsReturnValuesWithinBounds()
        {
            // Arrange
            double min = -1.0;
            double max = 1.0;
            var truncatedNormal = new NormalTruncated(0.0, 0.3, min, max);

            // Act & Assert
            for (int i = 0; i < 100; i++)
            {
                double result = truncatedNormal.Calculate();
                Assert.That(result, Is.GreaterThanOrEqualTo(min));
                Assert.That(result, Is.LessThanOrEqualTo(max));
            }
        }

        [Test]
        public void Calculate_WithTightBounds_StillReturnsValidValues()
        {
            // Arrange
            double min = 1.9;
            double max = 2.1;
            var truncatedNormal = new NormalTruncated(2.0, 0.5, min, max);

            // Act & Assert
            for (int i = 0; i < 50; i++)
            {
                double result = truncatedNormal.Calculate();
                Assert.That(result, Is.GreaterThanOrEqualTo(min));
                Assert.That(result, Is.LessThanOrEqualTo(max));
            }
        }

        [Test]
        public void Calculate_WithWideBounds_ReturnsVariedValues()
        {
            // Arrange
            var truncatedNormal = new NormalTruncated(0.0, 1.0, -10.0, 10.0);
            var results = new HashSet<double>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                results.Add(truncatedNormal.Calculate());
            }

            // Assert - should have variation due to randomness
            Assert.That(results.Count, Is.GreaterThan(1));
        }

        [Test]
        public void GetExpectedValue_AlwaysReturnsNaN()
        {
            // Arrange
            var truncatedNormal = new NormalTruncated(0, 1, -1, 1);

            // Act
            double result = truncatedNormal.GetExpectedValue();

            // Assert
            Assert.That(double.IsNaN(result), Is.True);
        }

        [Test]
        public void GetVariance_AlwaysReturnsNaN()
        {
            // Arrange
            var truncatedNormal = new NormalTruncated(0, 1, -1, 1);

            // Act
            double result = truncatedNormal.GetVariance();

            // Assert
            Assert.That(double.IsNaN(result), Is.True);
        }

        [Test]
        public void GetMinValue_ReturnsConstructorMin()
        {
            // Arrange
            double min = -2.5;
            var truncatedNormal = new NormalTruncated(0, 1, min, 2.5);

            // Act
            double result = truncatedNormal.GetMinValue();

            // Assert
            Assert.That(result, Is.EqualTo(min));
        }

        [Test]
        public void GetMaxValue_ReturnsConstructorMax()
        {
            // Arrange
            double max = 3.5;
            var truncatedNormal = new NormalTruncated(0, 1, -3.5, max);

            // Act
            double result = truncatedNormal.GetMaxValue();

            // Assert
            Assert.That(result, Is.EqualTo(max));
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var truncatedNormal = new NormalTruncated(2.5, 1.2, 0.5, 4.5);

            // Act
            string result = truncatedNormal.ToString();

            // Assert
            Assert.That(result, Does.Contain("TruncatedNormal"));
            Assert.That(result, Does.Contain("mean=2.5"));
            Assert.That(result, Does.Contain("std=1.2"));
            Assert.That(result, Does.Contain("min=0.5"));
            Assert.That(result, Does.Contain("max=4.5"));
        }

        [Test]
        public void ToString_FormatsNumbersWithThreeDecimals()
        {
            // Arrange
            var truncatedNormal = new NormalTruncated(1.23456, 0.98765, -2.12345, 3.45678);

            // Act
            string result = truncatedNormal.ToString();

            // Assert
            Assert.That(result, Does.Contain("mean=1.235"));
            Assert.That(result, Does.Contain("std=0.988"));
            Assert.That(result, Does.Contain("min=-2.123"));
            Assert.That(result, Does.Contain("max=3.457"));
        }

        [Test]
        public void Constructor_WithMinGreaterThanMax_ThrowsException()
        {
            // Arrange
            double min = 5.0;
            double max = 3.0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new NormalTruncated(0, 1, min, max));
        }

        [Test]
        public void Constructor_WithNegativeStandardDeviation_ThrowsException()
        {
            // Arrange
            double negativeStd = -1.0;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new NormalTruncated(0, negativeStd, -1, 1));
        }

        [Test]
        public void Calculate_WithMeanOutsideBounds_StillReturnsValuesWithinBounds()
        {
            // Arrange
            double mean = 5.0; // Outside bounds
            double min = 1.0;
            double max = 3.0;
            var truncatedNormal = new NormalTruncated(mean, 0.5, min, max);

            // Act & Assert
            for (int i = 0; i < 50; i++)
            {
                double result = truncatedNormal.Calculate();
                Assert.That(result, Is.GreaterThanOrEqualTo(min));
                Assert.That(result, Is.LessThanOrEqualTo(max));
            }
        }

        [Test]
        public void Calculate_WithZeroStandardDeviation_ReturnsValueWithinBounds()
        {
            // Arrange
            double mean = 2.0;
            double min = 1.0;
            double max = 3.0;
            var truncatedNormal = new NormalTruncated(mean, 0, min, max);

            // Act
            double result = truncatedNormal.Calculate();

            // Assert
            Assert.That(result, Is.EqualTo(mean).Within(Tolerance));
            Assert.That(result, Is.GreaterThanOrEqualTo(min));
            Assert.That(result, Is.LessThanOrEqualTo(max));
        }

        [Test]
        public void Calculate_WithMeanAtMinBound_ReturnsValidValues()
        {
            // Arrange
            double mean = 0.0;
            double min = 0.0;
            double max = 2.0;
            var truncatedNormal = new NormalTruncated(mean, 0.5, min, max);

            // Act & Assert
            for (int i = 0; i < 50; i++)
            {
                double result = truncatedNormal.Calculate();
                Assert.That(result, Is.GreaterThanOrEqualTo(min));
                Assert.That(result, Is.LessThanOrEqualTo(max));
            }
        }

        [Test]
        public void Calculate_WithMeanAtMaxBound_ReturnsValidValues()
        {
            // Arrange
            double mean = 2.0;
            double min = 0.0;
            double max = 2.0;
            var truncatedNormal = new NormalTruncated(mean, 0.5, min, max);

            // Act & Assert
            for (int i = 0; i < 50; i++)
            {
                double result = truncatedNormal.Calculate();
                Assert.That(result, Is.GreaterThanOrEqualTo(min));
                Assert.That(result, Is.LessThanOrEqualTo(max));
            }
        }
    }
