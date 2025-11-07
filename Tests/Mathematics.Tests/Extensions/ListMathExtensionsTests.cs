using Mathematics.Extensions;

namespace Mathematics.Tests.Extensions;

[TestFixture]
public class ListMathExtensionsTests
{
    private const double Tolerance = 1e-10;

    [TestFixture]
    public class VarianceTests
    {
        [Test]
        public void Variance_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            IList<double> emptyList = new List<double>();

            // Act & Assert
            Assert.That(() => emptyList.Variance(), Throws.ArgumentException);
        }

        [Test]
        public void Variance_NullList_ThrowsArgumentException()
        {
            // Arrange
            IList<double>? nullList = null;

            // Act & Assert
            Assert.That(() => nullList.Variance(), Throws.ArgumentException);
        }

        [Test]
        public void Variance_SingleElement_ReturnsZero()
        {
            // Arrange
            IList<double> singleElement = new List<double> { 5.0 };

            // Act
            var result = singleElement.Variance();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Variance_ConstantValues_ReturnsZero()
        {
            // Arrange
            IList<double> constantValues = new List<double> { 3.0, 3.0, 3.0, 3.0 };

            // Act
            var result = constantValues.Variance();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Variance_PositiveNumbers_ReturnsCorrectValue()
        {
            // Arrange
            IList<double> numbers = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 };
            const double expected = 2.0;

            // Act
            var result = numbers.Variance();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void Variance_NegativeNumbers_ReturnsCorrectValue()
        {
            // Arrange
            IList<double> numbers = new List<double> { -2.0, -1.0, 0.0, 1.0, 2.0 };
            var expected = 2.0;

            // Act
            var result = numbers.Variance();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }

        [Test]
        public void Variance_MixedNumbers_ReturnsCorrectValue()
        {
            // Arrange
            IList<double> numbers = new List<double> { -1.0, 0.0, 1.0 };
            var expected = 2.0 / 3.0;

            // Act
            var result = numbers.Variance();

            // Assert
            Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
        }
    }

    [TestFixture]
    public class StandardDeviationTests
    {
        [Test]
        public void StandardDeviation_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            IList<double> emptyList = new List<double>();

            // Act & Assert
            Assert.That(() => emptyList.StandardDeviation(), 
                Throws.ArgumentException);
        }

        [Test]
        public void StandardDeviation_SingleElement_ReturnsZero()
        {
            // Arrange
            IList<double> singleElement = new List<double> { 7.0 };

            // Act
            var result = singleElement.StandardDeviation();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void StandardDeviation_ConstantValues_ReturnsZero()
        {
            // Arrange
            IList<double> constantValues = new List<double> { 2.5, 2.5, 2.5 };

            // Act
            var result = constantValues.StandardDeviation();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void StandardDeviation_PositiveNumbers_ReturnsCorrectValue()
        {
            // Arrange
            IList<double> numbers = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 };
            const double expectedVariance = 2.0;
            var expectedStdDev = Math.Sqrt(expectedVariance);

            // Act
            var result = numbers.StandardDeviation();

            // Assert
            Assert.That(result, Is.EqualTo(expectedStdDev).Within(Tolerance));
        }

        [Test]
        public void StandardDeviation_RelationshipWithVariance()
        {
            // Arrange
            IList<double> numbers = new List<double> { 10.0, 20.0, 30.0, 40.0 };

            // Act
            var variance = numbers.Variance();
            var stdDev = numbers.StandardDeviation();

            // Assert
            Assert.That(stdDev * stdDev, Is.EqualTo(variance).Within(Tolerance));
        }
    }

    [TestFixture]
    public class SkewnessTests
    {
        [Test]
        public void Skewness_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            IList<double> emptyList = new List<double>();

            // Act & Assert
            Assert.That(() => emptyList.Skewness(), 
                Throws.ArgumentException);
        }

        [Test]
        public void Skewness_LessThanThreeElements_ThrowsArgumentException()
        {
            // Arrange
            IList<double> twoElements = new List<double> { 1.0, 2.0 };

            // Act & Assert
            Assert.That(() => twoElements.Skewness(), 
                Throws.ArgumentException);
        }

        [Test]
        public void Skewness_ConstantValues_ReturnsZero()
        {
            // Arrange
            IList<double> constantValues = new List<double> { 5.0, 5.0, 5.0 };

            // Act
            var result = constantValues.Skewness();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Skewness_SymmetricDistribution_ReturnsZero()
        {
            // Arrange
            IList<double> symmetricData = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 };

            // Act
            var result = symmetricData.Skewness();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Skewness_RightSkewed_ReturnsPositiveValue()
        {
            // Arrange
            IList<double> rightSkewed = new List<double> { 1.0, 2.0, 3.0, 4.0, 10.0 };

            // Act
            var result = rightSkewed.Skewness();

            // Assert
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void Skewness_LeftSkewed_ReturnsNegativeValue()
        {
            // Arrange
            IList<double> leftSkewed = new List<double> { 1.0, 8.0, 9.0, 10.0, 10.0 };

            // Act
            var result = leftSkewed.Skewness();

            // Assert
            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void Skewness_ZeroStandardDeviation_ReturnsZero()
        {
            // Arrange
            IList<double> constantWithThree = new List<double> { 7.0, 7.0, 7.0 };

            // Act
            var result = constantWithThree.Skewness();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }
    }

    [TestFixture]
    public class MedianTests
    {
        [Test]
        public void Median_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            IList<double> emptyList = new List<double>();

            // Act & Assert
            Assert.That(() => emptyList.Median(), 
                Throws.ArgumentException);
        }

        [Test]
        public void Median_SingleElement_ReturnsElement()
        {
            // Arrange
            IList<double> singleElement = new List<double> { 42.0 };

            // Act
            var result = singleElement.Median();

            // Assert
            Assert.That(result, Is.EqualTo(42.0).Within(Tolerance));
        }

        [Test]
        public void Median_OddNumberOfElements_ReturnsMiddleElement()
        {
            // Arrange
            IList<double> oddCount = new List<double> { 1.0, 3.0, 2.0 }; // Sorted: 1, 2, 3

            // Act
            var result = oddCount.Median();

            // Assert
            Assert.That(result, Is.EqualTo(2.0).Within(Tolerance));
        }

        [Test]
        public void Median_EvenNumberOfElements_ReturnsAverageOfMiddleTwo()
        {
            // Arrange
            IList<double> evenCount = new List<double> { 1.0, 4.0, 2.0, 3.0 }; // Sorted: 1, 2, 3, 4

            // Act
            var result = evenCount.Median();

            // Assert
            Assert.That(result, Is.EqualTo(2.5).Within(Tolerance));
        }

        [Test]
        public void Median_UnsortedList_ReturnsCorrectMedian()
        {
            // Arrange
            IList<double> unsorted = new List<double> { 5.0, 1.0, 3.0, 2.0, 4.0 };

            // Act
            var result = unsorted.Median();

            // Assert
            Assert.That(result, Is.EqualTo(3.0).Within(Tolerance));
        }

        [Test]
        public void Median_NegativeNumbers_ReturnsCorrectMedian()
        {
            // Arrange
            IList<double> negativeNumbers = new List<double> { -5.0, -1.0, -3.0 };

            // Act
            var result = negativeNumbers.Median();

            // Assert
            Assert.That(result, Is.EqualTo(-3.0).Within(Tolerance));
        }
    }

    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void Range_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            IList<double> emptyList = new List<double>();

            // Act & Assert
            Assert.That(() => emptyList.Range(), 
                Throws.ArgumentException);
        }

        [Test]
        public void Range_SingleElement_ReturnsZero()
        {
            // Arrange
            IList<double> singleElement = new List<double> { 7.5 };

            // Act
            var result = singleElement.Range();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Range_PositiveNumbers_ReturnsCorrectValue()
        {
            // Arrange
            IList<double> numbers = new List<double> { 1.0, 5.0, 3.0, 9.0, 2.0 };

            // Act
            var result = numbers.Range();

            // Assert
            Assert.That(result, Is.EqualTo(8.0).Within(Tolerance)); // 9 - 1 = 8
        }

        [Test]
        public void Range_NegativeNumbers_ReturnsCorrectValue()
        {
            // Arrange
            IList<double> numbers = new List<double> { -5.0, -1.0, -3.0 };

            // Act
            var result = numbers.Range();

            // Assert
            Assert.That(result, Is.EqualTo(4.0).Within(Tolerance)); // -1 - (-5) = 4
        }

        [Test]
        public void Range_MixedNumbers_ReturnsCorrectValue()
        {
            // Arrange
            IList<double> numbers = new List<double> { -2.0, 0.0, 3.0, -1.0 };

            // Act
            var result = numbers.Range();

            // Assert
            Assert.That(result, Is.EqualTo(5.0).Within(Tolerance)); // 3 - (-2) = 5
        }
    }

    [TestFixture]
    public class KurtosisTests
    {
        [Test]
        public void Kurtosis_EmptyList_ThrowsArgumentException()
        {
            // Arrange
            IList<double> emptyList = new List<double>();

            // Act & Assert
            Assert.That(() => emptyList.Kurtosis(), 
                Throws.ArgumentException);
        }

        [Test]
        public void Kurtosis_LessThanFourElements_ThrowsArgumentException()
        {
            // Arrange
            IList<double> threeElements = new List<double> { 1.0, 2.0, 3.0 };

            // Act & Assert
            Assert.That(() => threeElements.Kurtosis(), 
                Throws.ArgumentException);
        }

        [Test]
        public void Kurtosis_ConstantValues_ReturnsZero()
        {
            // Arrange
            IList<double> constantValues = new List<double> { 4.0, 4.0, 4.0, 4.0 };

            // Act
            var result = constantValues.Kurtosis();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Kurtosis_NormalDistributionLike_ReturnsNearZero()
        {
            // Arrange
            IList<double> normalLike = new List<double> { -1.0, 0.0, 0.0, 1.0 };

            // Act
            var result = normalLike.Kurtosis();

            // Assert - для небольшой выборки значение может отличаться
            Assert.That(Math.Abs(result), Is.LessThan(2.0));
        }

        [Test]
        public void Kurtosis_ZeroStandardDeviation_ReturnsZero()
        {
            // Arrange
            IList<double> constantWithFour = new List<double> { 2.0, 2.0, 2.0, 2.0 };

            // Act
            var result = constantWithFour.Kurtosis();

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Kurtosis_LeptokurticDistribution_ReturnsPositiveValue()
        {
            // Arrange
            // Лептокуртическое распределение с тяжелыми хвостами - больше экстремальных значений
            IList<double> leptokurtic = new List<double> { -5.0, -4.0, 0.0, 0.0, 0.0, 0.0, 0.0, 4.0, 5.0 };

            // Act
            var result = leptokurtic.Kurtosis();

            // Assert - положительное значение указывает на тяжелые хвосты
            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void Kurtosis_PlatykurticDistribution_ReturnsNegativeValue()
        {
            // Arrange
            // Платикуртическое распределение с легкими хвостами - равномерное распределение
            IList<double> platykurtic = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0 };

            // Act
            var result = platykurtic.Kurtosis();

            // Assert - отрицательное значение указывает на легкие хвосты
            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void Kurtosis_MesokurticDistribution_ReturnsNearZero()
        {
            // Arrange
            // Мезокуртическое распределение (нормальное) - эксцесс близок к 0
            IList<double> mesokurtic = new List<double> { -2.0, -1.0, 0.0, 0.0, 0.0, 1.0, 2.0 };

            // Act
            var result = mesokurtic.Kurtosis();

            // Assert - значение близко к 0
            Assert.That(result, Is.EqualTo(0).Within(1.0));
        }

        [Test]
        public void Kurtosis_VeryLeptokurtic_ReturnsHighPositiveValue()
        {
            // Arrange
            // Очень лептокуртическое распределение с очень тяжелыми хвостами
            IList<double> veryLeptokurtic = new List<double> 
            { 
                -10.0, -10.0, -5.0, 0.0, 0.0, 0.0, 0.0, 0.0, 5.0, 10.0, 10.0 
            };

            // Act
            var result = veryLeptokurtic.Kurtosis();

            // Assert
            Assert.That(result, Is.GreaterThan(1.0));
        }
    }

    [TestFixture]
    public class IntegrationTests
    {
        [Test]
        public void MultipleStatistics_RealDataset_ReturnsConsistentResults()
        {
            // Arrange
            IList<double> dataset = new List<double> { 10.0, 20.0, 30.0, 40.0, 50.0 };

            // Act
            var mean = dataset.Average();
            var variance = dataset.Variance();
            var stdDev = dataset.StandardDeviation();
            var median = dataset.Median();
            var range = dataset.Range();
            var skewness = dataset.Skewness();
            var kurtosis = dataset.Kurtosis();

            // Assert - проверка консистентности результатов
            Assert.That(stdDev * stdDev, Is.EqualTo(variance).Within(Tolerance));
            Assert.That(median, Is.EqualTo(30.0).Within(Tolerance));
            Assert.That(range, Is.EqualTo(40.0).Within(Tolerance));
            Assert.That(skewness, Is.EqualTo(0).Within(0.1)); // Симметричные данные
        }

        [Test]
        public void LargeDataset_PerformanceAndCorrectness()
        {
            // Arrange
            var random = new Random(42);
            IList<double> largeDataset = Enumerable.Range(0, 1000)
                .Select(_ => random.NextDouble() * 100)
                .ToList();

            // Act
            var variance = largeDataset.Variance();
            var stdDev = largeDataset.StandardDeviation();
            var median = largeDataset.Median();
            var range = largeDataset.Range();

            // Assert - проверка что вычисления завершаются и дают разумные результаты
            Assert.That(variance, Is.GreaterThan(0));
            Assert.That(stdDev, Is.GreaterThan(0));
            Assert.That(median, Is.InRange(0, 100));
            Assert.That(range, Is.InRange(0, 100));
        }
    }
}