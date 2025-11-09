using Mathematics.Statistics.Extensions;

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
			
			List<double> emptyList = [];

			 
			Assert.That(() => emptyList.Variance(), Throws.ArgumentException);
		}

		[Test]
		public void Variance_NullList_ThrowsArgumentException()
		{
			
			List<double>? nullList = null;

			 
			Assert.That(() => nullList.Variance(), Throws.ArgumentException);
		}

		[Test]
		public void Variance_SingleElement_ReturnsZero()
		{
			
			List<double> singleElement = [5.0];

			
			var result = singleElement.Variance();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void Variance_ConstantValues_ReturnsZero()
		{
			
			List<double> constantValues = [3.0, 3.0, 3.0, 3.0];

			
			var result = constantValues.Variance();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void Variance_PositiveNumbers_ReturnsCorrectValue()
		{
			
			List<double> numbers = [1.0, 2.0, 3.0, 4.0, 5.0];
			const double expected = 2.0;

			
			var result = numbers.Variance();

			
			Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
		}

		[Test]
		public void Variance_NegativeNumbers_ReturnsCorrectValue()
		{
			
			List<double> numbers = [-2.0, -1.0, 0.0, 1.0, 2.0];
			var expected = 2.0;

			
			var result = numbers.Variance();

			
			Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
		}

		[Test]
		public void Variance_MixedNumbers_ReturnsCorrectValue()
		{
			
			List<double> numbers = [-1.0, 0.0, 1.0];
			var expected = 2.0 / 3.0;

			
			var result = numbers.Variance();

			
			Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
		}
	}

	[TestFixture]
	public class StandardDeviationTests
	{
		[Test]
		public void StandardDeviation_EmptyList_ThrowsArgumentException()
		{
			
			List<double> emptyList = [];

			 
			Assert.That(() => emptyList.StandardDeviation(),
				Throws.ArgumentException);
		}

		[Test]
		public void StandardDeviation_SingleElement_ReturnsZero()
		{
			
			List<double> singleElement = [7.0];

			
			var result = singleElement.StandardDeviation();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void StandardDeviation_ConstantValues_ReturnsZero()
		{
			
			List<double> constantValues = [2.5, 2.5, 2.5];

			
			var result = constantValues.StandardDeviation();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void StandardDeviation_PositiveNumbers_ReturnsCorrectValue()
		{
			
			List<double> numbers = [1.0, 2.0, 3.0, 4.0, 5.0];
			const double expectedVariance = 2.0;
			var expectedStdDev = Math.Sqrt(expectedVariance);

			
			var result = numbers.StandardDeviation();

			
			Assert.That(result, Is.EqualTo(expectedStdDev).Within(Tolerance));
		}

		[Test]
		public void StandardDeviation_RelationshipWithVariance()
		{
			
			List<double> numbers = [10.0, 20.0, 30.0, 40.0];

			
			var variance = numbers.Variance();
			var stdDev = numbers.StandardDeviation();

			
			Assert.That(stdDev * stdDev, Is.EqualTo(variance).Within(Tolerance));
		}
	}

	[TestFixture]
	public class SkewnessTests
	{
		[Test]
		public void Skewness_EmptyList_ThrowsArgumentException()
		{
			
			List<double> emptyList = [];

			 
			Assert.That(() => emptyList.Skewness(),
				Throws.ArgumentException);
		}

		[Test]
		public void Skewness_LessThanThreeElements_ThrowsArgumentException()
		{
			
			List<double> twoElements = [1.0, 2.0];

			 
			Assert.That(() => twoElements.Skewness(),
				Throws.ArgumentException);
		}

		[Test]
		public void Skewness_ConstantValues_ReturnsZero()
		{
			List<double> constantValues = [5.0, 5.0, 5.0];
			
			var result = constantValues.Skewness();
			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void Skewness_SymmetricDistribution_ReturnsZero()
		{
			List<double> symmetricData = [1.0, 2.0, 3.0, 4.0, 5.0];
			
			var result = symmetricData.Skewness();
			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void Skewness_RightSkewed_ReturnsPositiveValue()
		{
			List<double> rightSkewed = [1.0, 2.0, 3.0, 4.0, 10.0];
			
			var result = rightSkewed.Skewness();
			
			Assert.That(result, Is.GreaterThan(0));
		}

		[Test]
		public void Skewness_LeftSkewed_ReturnsNegativeValue()
		{
			List<double> leftSkewed = [1.0, 8.0, 9.0, 10.0, 10.0];
			
			var result = leftSkewed.Skewness();
			
			Assert.That(result, Is.LessThan(0));
		}

		[Test]
		public void Skewness_ZeroStandardDeviation_ReturnsZero()
		{
			
			List<double> constantWithThree = [7.0, 7.0, 7.0];

			
			var result = constantWithThree.Skewness();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}
	}

	[TestFixture]
	public class MedianTests
	{
		[Test]
		public void Median_EmptyList_ThrowsArgumentException()
		{
			
			List<double> emptyList = [];

			 
			Assert.That(() => emptyList.Median(),
				Throws.ArgumentException);
		}

		[Test]
		public void Median_SingleElement_ReturnsElement()
		{
			
			List<double> singleElement = [42.0];

			
			var result = singleElement.Median();

			
			Assert.That(result, Is.EqualTo(42.0).Within(Tolerance));
		}

		[Test]
		public void Median_OddNumberOfElements_ReturnsMiddleElement()
		{
			
			List<double> oddCount = [1.0, 3.0, 2.0]; // Sorted: 1, 2, 3

			
			var result = oddCount.Median();

			
			Assert.That(result, Is.EqualTo(2.0).Within(Tolerance));
		}

		[Test]
		public void Median_EvenNumberOfElements_ReturnsAverageOfMiddleTwo()
		{
			
			List<double> evenCount = [1.0, 4.0, 2.0, 3.0]; // Sorted: 1, 2, 3, 4

			
			var result = evenCount.Median();

			
			Assert.That(result, Is.EqualTo(2.5).Within(Tolerance));
		}

		[Test]
		public void Median_UnsortedList_ReturnsCorrectMedian()
		{
			
			List<double> unsorted = [5.0, 1.0, 3.0, 2.0, 4.0];

			
			var result = unsorted.Median();

			
			Assert.That(result, Is.EqualTo(3.0).Within(Tolerance));
		}

		[Test]
		public void Median_NegativeNumbers_ReturnsCorrectMedian()
		{
			
			List<double> negativeNumbers = [-5.0, -1.0, -3.0];

			
			var result = negativeNumbers.Median();

			
			Assert.That(result, Is.EqualTo(-3.0).Within(Tolerance));
		}
	}

	[TestFixture]
	public class RangeTests
	{
		[Test]
		public void Range_EmptyList_ThrowsArgumentException()
		{
			
			List<double> emptyList = [];

			 
			Assert.That(() => emptyList.Range(),
				Throws.ArgumentException);
		}

		[Test]
		public void Range_SingleElement_ReturnsZero()
		{
			
			List<double> singleElement = [7.5];

			
			var result = singleElement.Range();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void Range_PositiveNumbers_ReturnsCorrectValue()
		{
			
			List<double> numbers = [1.0, 5.0, 3.0, 9.0, 2.0];

			
			var result = numbers.Range();

			
			Assert.That(result, Is.EqualTo(8.0).Within(Tolerance));
		}

		[Test]
		public void Range_NegativeNumbers_ReturnsCorrectValue()
		{
			
			List<double> numbers = [-5.0, -1.0, -3.0];

			
			var result = numbers.Range();

			
			Assert.That(result, Is.EqualTo(4.0).Within(Tolerance));
		}

		[Test]
		public void Range_MixedNumbers_ReturnsCorrectValue()
		{
			
			List<double> numbers = [-2.0, 0.0, 3.0, -1.0];

			
			var result = numbers.Range();

			
			Assert.That(result, Is.EqualTo(5.0).Within(Tolerance));
		}
	}

	[TestFixture]
	public class KurtosisTests
	{
		[Test]
		public void Kurtosis_EmptyList_ThrowsArgumentException()
		{
			
			List<double> emptyList = [];

			 
			Assert.That(() => emptyList.Kurtosis(),
				Throws.ArgumentException);
		}

		[Test]
		public void Kurtosis_LessThanFourElements_ThrowsArgumentException()
		{
			
			List<double> threeElements = [1.0, 2.0, 3.0];

			 
			Assert.That(() => threeElements.Kurtosis(),
				Throws.ArgumentException);
		}

		[Test]
		public void Kurtosis_ConstantValues_ReturnsZero()
		{
			
			List<double> constantValues = [4.0, 4.0, 4.0, 4.0];

			
			var result = constantValues.Kurtosis();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void Kurtosis_NormalDistributionLike_ReturnsNearZero()
		{
			
			List<double> normalLike = [-1.0, 0.0, 0.0, 1.0];

			
			var result = normalLike.Kurtosis();

			Assert.That(Math.Abs(result), Is.LessThan(2.0));
		}

		[Test]
		public void Kurtosis_ZeroStandardDeviation_ReturnsZero()
		{
			
			List<double> constantWithFour = [2.0, 2.0, 2.0, 2.0];

			
			var result = constantWithFour.Kurtosis();

			
			Assert.That(result, Is.EqualTo(0).Within(Tolerance));
		}

		[Test]
		public void Kurtosis_LeptokurticDistribution_ReturnsPositiveValue()
		{
			List<double> leptokurtic = [-5.0, -4.0, 0.0, 0.0, 0.0, 0.0, 0.0, 4.0, 5.0];
			
			var result = leptokurtic.Kurtosis();

			Assert.That(result, Is.GreaterThan(0));
		}

		[Test]
		public void Kurtosis_PlatykurticDistribution_ReturnsNegativeValue()
		{
			List<double> platykurtic = [1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0];
			
			var result = platykurtic.Kurtosis();

			Assert.That(result, Is.LessThan(0));
		}

		[Test]
		public void Kurtosis_MesokurticDistribution_ReturnsNearZero()
		{
			
			List<double> mesokurtic = [-2.0, -1.0, 0.0, 0.0, 0.0, 1.0, 2.0];
			
			var result = mesokurtic.Kurtosis();

			Assert.That(result, Is.EqualTo(0).Within(1.0));
		}

		[Test]
		public void Kurtosis_VeryLeptokurtic_ReturnsHighPositiveValue()
		{
			List<double> veryLeptokurtic = [-10.0, -10.0, -5.0, 0.0, 0.0, 0.0, 0.0, 0.0, 5.0, 10.0, 10.0];
			
			var result = veryLeptokurtic.Kurtosis();

			
			Assert.That(result, Is.GreaterThan(1.0));
		}
	}

	[TestFixture]
	public class IntegrationTests
	{
		[Test]
		public void MultipleStatistics_RealDataset_ReturnsConsistentResults()
		{
			List<double> dataset = [10.0, 20.0, 30.0, 40.0, 50.0];
			
			var mean = dataset.Average();
			var variance = dataset.Variance();
			var stdDev = dataset.StandardDeviation();
			var median = dataset.Median();
			var range = dataset.Range();
			var skewness = dataset.Skewness();
			var kurtosis = dataset.Kurtosis();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stdDev * stdDev, Is.EqualTo(variance).Within(Tolerance));
                Assert.That(median, Is.EqualTo(30.0).Within(Tolerance));
                Assert.That(range, Is.EqualTo(40.0).Within(Tolerance));
                Assert.That(skewness, Is.EqualTo(0).Within(0.1));
            }
        }

		[Test]
		public void LargeDataset_PerformanceAndCorrectness()
		{
			var random = new Random(42);
			var largeDataset = Enumerable.Range(0, 1000)
				.Select(_ => random.NextDouble() * 100)
				.ToList();
			
			var variance = largeDataset.Variance();
			var stdDev = largeDataset.StandardDeviation();
			var median = largeDataset.Median();
			var range = largeDataset.Range();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(variance, Is.GreaterThan(0));
                Assert.That(stdDev, Is.GreaterThan(0));
                Assert.That(median, Is.InRange(0, 100));
                Assert.That(range, Is.InRange(0, 100));
            }
        }
	}
}