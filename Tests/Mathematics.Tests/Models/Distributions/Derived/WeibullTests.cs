using Mathematics.Models.Distributions.Derived;
using Mathematics.Utilities.Formulas;

namespace Mathematics.Tests.Models.Distributions.Derived
{
    [TestFixture]
    public class WeibullTests
    {
        private const double Tolerance = 0.001;

        [Test]
        public void Constructor_ValidParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            double scale = 2.5;
            double shape = 1.2;

            // Act
            var weibull = new Weibull(scale, shape);

            // Assert
            Assert.That(weibull.Scale, Is.EqualTo(scale));
            Assert.That(weibull.Shape, Is.EqualTo(shape));
        }

        [Test]
        public void Constructor_NegativeScale_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            double negativeScale = -1.0;
            double shape = 1.0;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Weibull(negativeScale, shape));
        }

        [Test]
        public void Constructor_NegativeShape_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            double scale = 1.0;
            double negativeShape = -1.0;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new Weibull(scale, negativeShape));
        }

        [Test]
        public void Constructor_ZeroScale_Allowed()
        {
            // Arrange
            double scale = 0.0;
            double shape = 1.0;

            // Act
            var weibull = new Weibull(scale, shape);

            // Assert
            Assert.That(weibull.Scale, Is.EqualTo(scale));
        }

        [Test]
        public void Constructor_ZeroShape_Allowed()
        {
            // Arrange
            double scale = 1.0;
            double shape = 0.0;

            // Act
            var weibull = new Weibull(scale, shape);

            // Assert
            Assert.That(weibull.Shape, Is.EqualTo(shape));
        }

        [Test]
        public void Calculate_ReturnsNonNegativeValue()
        {
            // Arrange
            var weibull = new Weibull(1.0, 1.0);

            // Act
            double result = weibull.Calculate();

            // Assert
            Assert.That(result, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void Calculate_MultipleCallsReturnDifferentValues()
        {
            // Arrange
            var weibull = new Weibull(2.0, 1.5);
            var results = new HashSet<double>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                results.Add(weibull.Calculate());
            }

            // Assert - should have some variation due to randomness
            Assert.That(results.Count, Is.GreaterThan(1));
        }

        [Test]
        public void Calculate_WithZeroScale_ReturnsZero()
        {
            // Arrange
            var weibull = new Weibull(0.0, 1.0);

            // Act
            double result = weibull.Calculate();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Calculate_WithShapeOne_ReturnsExponentialDistribution()
        {
            // Arrange
            var weibull = new Weibull(2.0, 1.0);

            // Act
            double result = weibull.Calculate();

            // Assert - should be positive
            Assert.That(result, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetExpectedValue_CalculatesCorrectly()
        {
            // Arrange
            double scale = 2.0;
            double shape = 1.5;
            var weibull = new Weibull(scale, shape);
            double expected = scale * GammaUtils.Gamma(1.0 + 1.0 / shape);

            // Act
            double result = weibull.GetExpectedValue();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetExpectedValue_WithShapeOne_ReturnsScale()
        {
            // Arrange
            double scale = 3.0;
            var weibull = new Weibull(scale, 1.0);
            double expected = scale; // Gamma(2) = 1!

            // Act
            double result = weibull.GetExpectedValue();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetExpectedValue_WithZeroScale_ReturnsZero()
        {
            // Arrange
            var weibull = new Weibull(0.0, 2.0);

            // Act
            double result = weibull.GetExpectedValue();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetVariance_CalculatesCorrectly()
        {
            // Arrange
            double scale = 2.0;
            double shape = 1.5;
            var weibull = new Weibull(scale, shape);
            double gamma1 = GammaUtils.Gamma(1.0 + 1.0 / shape);
            double gamma2 = GammaUtils.Gamma(1.0 + 2.0 / shape);
            double expected = scale * scale * (gamma2 - gamma1 * gamma1);

            // Act
            double result = weibull.GetVariance();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetVariance_WithShapeOne_ReturnsScaleSquared()
        {
            // Arrange
            double scale = 3.0;
            var weibull = new Weibull(scale, 1.0);
            double expected = scale * scale; // Gamma(3) = 2!, Gamma(2) = 1! => 2 - 1 = 1

            // Act
            double result = weibull.GetVariance();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void GetVariance_WithZeroScale_ReturnsZero()
        {
            // Arrange
            var weibull = new Weibull(0.0, 2.0);

            // Act
            double result = weibull.GetVariance();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetVariance_IsAlwaysNonNegative()
        {
            // Arrange
            var testCases = new[]
            {
                new { Scale = 1.0, Shape = 0.5 },
                new { Scale = 2.0, Shape = 1.0 },
                new { Scale = 3.0, Shape = 2.0 },
                new { Scale = 0.5, Shape = 3.0 }
            };

            foreach (var testCase in testCases)
            {
                var weibull = new Weibull(testCase.Scale, testCase.Shape);

                // Act
                double variance = weibull.GetVariance();

                // Assert
                Assert.That(variance, Is.GreaterThanOrEqualTo(0), 
                    $"Variance should be non-negative for scale={testCase.Scale}, shape={testCase.Shape}");
            }
        }

        [Test]
        public void GetMinValue_AlwaysReturnsZero()
        {
            // Arrange
            var weibull = new Weibull(1.0, 1.0);

            // Act
            double result = weibull.GetMinValue();

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetMaxValue_AlwaysReturnsPositiveInfinity()
        {
            // Arrange
            var weibull = new Weibull(1.0, 1.0);

            // Act
            double result = weibull.GetMaxValue();

            // Assert
            Assert.That(result, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var weibull = new Weibull(2.5, 1.2);

            // Act
            string result = weibull.ToString();

            // Assert
            Assert.That(result, Does.Contain("Weibull"));
            Assert.That(result, Does.Contain("Scale = 2.5"));
            Assert.That(result, Does.Contain("Shape = 1.2"));
            Assert.That(result, Does.Match(@"Weibull \[Scale = \d+\.\d+, Shape = \d+\.\d+\]"));
        }

        [Test]
        public void ToString_FormatsNumbersWithThreeDecimals()
        {
            // Arrange
            var weibull = new Weibull(1.23456, 0.98765);

            // Act
            string result = weibull.ToString();

            // Assert
            Assert.That(result, Does.Contain("Scale = 1.235"));
            Assert.That(result, Does.Contain("Shape = 0.988"));
        }

        [Test]
        public void Calculate_WithLargeShape_ReturnsValuesCloseToScale()
        {
            // Arrange
            double scale = 2.0;
            double largeShape = 100.0;
            var weibull = new Weibull(scale, largeShape);

            // Act & Assert
            for (int i = 0; i < 50; i++)
            {
                double result = weibull.Calculate();
                // With large shape, distribution becomes concentrated near scale
                Assert.That(result, Is.GreaterThanOrEqualTo(0));
                Assert.That(result, Is.LessThanOrEqualTo(scale * 2)); // Reasonable upper bound
            }
        }

        [Test]
        public void Calculate_WithSmallShape_ReturnsWideRangeOfValues()
        {
            // Arrange
            var weibull = new Weibull(1.0, 0.5);
            var results = new List<double>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                results.Add(weibull.Calculate());
            }

            // Assert - should have large variation
            results.Sort();
            double range = results[^1] - results[0];
            Assert.That(range, Is.GreaterThan(1.0)); // Significant range
        }

        [Test]
        public void ExpectedValue_WithDifferentParameters_ReturnsPositiveValues()
        {
            // Arrange
            var testCases = new[]
            {
                new { Scale = 0.5, Shape = 0.8 },
                new { Scale = 1.0, Shape = 1.0 },
                new { Scale = 2.0, Shape = 1.5 },
                new { Scale = 5.0, Shape = 3.0 }
            };

            foreach (var testCase in testCases)
            {
                var weibull = new Weibull(testCase.Scale, testCase.Shape);

                // Act
                double expectedValue = weibull.GetExpectedValue();

                // Assert
                Assert.That(expectedValue, Is.GreaterThan(0), 
                    $"Expected value should be positive for scale={testCase.Scale}, shape={testCase.Shape}");
            }
        }
    }
}