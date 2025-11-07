using Mathematics.Models.Distributions.Derived;

namespace Mathematics.Tests.Models.Distributions.Derived;

using NUnit.Framework;
using System;
using System.Collections.Generic;

    [TestFixture]
    public class NormalLogTests
    {
        private const double Tolerance = 0.001;

        [Test]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            double mean = 2.5;
            double standardDeviation = 1.2;

            // Act
            var normalLog = new NormalLog(mean, standardDeviation);

            // Assert
            Assert.That(normalLog.Mean, Is.EqualTo(mean));
            Assert.That(normalLog.StandardDeviation, Is.EqualTo(standardDeviation));
        }

        [Test]
        public void Calculate_ReturnsPositiveValue()
        {
            // Arrange
            var normalLog = new NormalLog(0, 1);

            // Act
            double result = normalLog.Calculate();

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void Calculate_MultipleCallsReturnDifferentValues()
        {
            // Arrange
            var normalLog = new NormalLog(0, 1);
            var results = new HashSet<double>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                results.Add(normalLog.Calculate());
            }

            // Assert - should have some variation due to randomness
            Assert.That(results.Count, Is.GreaterThan(1));
        }

        [Test]
        public void GetExpectedValue_CalculatesCorrectly()
        {
            // Arrange
            double mean = 1.0;
            double std = 0.5;
            var normalLog = new NormalLog(mean, std);
            double expected = Math.Exp(mean + std * std / 2.0);

            // Act
            double result = normalLog.GetExpectedValue();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetExpectedValue_WithZeroMeanAndStd_ReturnsOne()
        {
            // Arrange
            var normalLog = new NormalLog(0, 0);
            double expected = 1.0;

            // Act
            double result = normalLog.GetExpectedValue();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetVariance_CalculatesCorrectly()
        {
            // Arrange
            double mean = 1.0;
            double std = 0.5;
            var normalLog = new NormalLog(mean, std);
            double expected = Math.Exp(2 * mean + std * std) * (Math.Exp(std * std) - 1);

            // Act
            double result = normalLog.GetVariance();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetVariance_WithZeroMeanAndStd_ReturnsZero()
        {
            // Arrange
            var normalLog = new NormalLog(0, 0);
            double expected = 0.0;

            // Act
            double result = normalLog.GetVariance();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetMinValue_AlwaysReturnsZero()
        {
            // Arrange
            var normalLog = new NormalLog(0, 1);

            // Act
            double result = normalLog.GetMinValue();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetMaxValue_AlwaysReturnsPositiveInfinity()
        {
            // Arrange
            var normalLog = new NormalLog(0, 1);

            // Act
            double result = normalLog.GetMaxValue();

            // Assert
            Assert.That(result, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var normalLog = new NormalLog(2.5, 1.2);

            // Act
            string result = normalLog.ToString();

            // Assert
            Assert.That(result, Does.Contain("LogNormal"));
            Assert.That(result, Does.Contain("Mean = 2.5"));
            Assert.That(result, Does.Contain("std=1.2"));
            Assert.That(result, Does.Match(@"LogNormal \[Mean = \d+\.\d+, std=\d+\.\d+\)"));
        }

        [Test]
        public void ToString_FormatsNumbersWithThreeDecimals()
        {
            // Arrange
            var normalLog = new NormalLog(1.23456, 0.98765);

            // Act
            string result = normalLog.ToString();

            // Assert
            Assert.That(result, Does.Contain("Mean = 1.235"));
            Assert.That(result, Does.Contain("std=0.988"));
        }

        // Тесты для проверки математических свойств логнормального распределения
        [Test]
        public void ExpectedValue_IsAlwaysPositive()
        {
            // Arrange
            var testCases = new[]
            {
                new { Mean = -1.0, Std = 0.5 },
                new { Mean = 0.0, Std = 1.0 },
                new { Mean = 1.0, Std = 2.0 }
            };

            foreach (var testCase in testCases)
            {
                var normalLog = new NormalLog(testCase.Mean, testCase.Std);

                // Act
                double expectedValue = normalLog.GetExpectedValue();

                // Assert
                Assert.That(expectedValue, Is.GreaterThan(0), 
                    $"Expected value should be positive for mean={testCase.Mean}, std={testCase.Std}");
            }
        }

        [Test]
        public void Variance_IsAlwaysNonNegative()
        {
            // Arrange
            var testCases = new[]
            {
                new { Mean = -1.0, Std = 0.5 },
                new { Mean = 0.0, Std = 1.0 },
                new { Mean = 1.0, Std = 2.0 }
            };

            foreach (var testCase in testCases)
            {
                var normalLog = new NormalLog(testCase.Mean, testCase.Std);

                // Act
                double variance = normalLog.GetVariance();

                // Assert
                Assert.That(variance, Is.GreaterThanOrEqualTo(0), 
                    $"Variance should be non-negative for mean={testCase.Mean}, std={testCase.Std}");
            }
        }
    }