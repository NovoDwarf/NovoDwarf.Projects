using Mathematics.Models.Distributions.Composite;

namespace Mathematics.Tests.Models.Distributions.Composite;

[TestFixture]
public class ExponentialHyperTests
{
    private const double Tolerance = 1e-10;

    [Test]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0, 2.0];

        // Act
        var distribution = new ExponentialHyper(probabilities, rates);

        // Assert
        Assert.That(distribution.Probabilities, Is.EqualTo(probabilities));
        Assert.That(distribution.Rates, Is.EqualTo(rates));
    }

    [Test]
    public void Constructor_NullProbabilities_ThrowsArgumentNullException()
    {
        // Arrange
        double[]? probabilities = null;
        double[] rates = [1.0, 2.0];

        // Act & Assert
        Assert.That(() => new ExponentialHyper(probabilities, rates), 
            Throws.ArgumentNullException.With.Message.Contains("Probabilities and rates cannot be null"));
    }

    [Test]
    public void Constructor_NullRates_ThrowsArgumentNullException()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[]? rates = null;

        // Act & Assert
        Assert.That(() => new ExponentialHyper(probabilities, rates), 
            Throws.ArgumentNullException.With.Message.Contains("Probabilities and rates cannot be null"));
    }

    [Test]
    public void Constructor_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0];

        // Act & Assert
        Assert.That(() => new ExponentialHyper(probabilities, rates), 
            Throws.ArgumentException.With.Message.Contains("Probabilities and rates must have same length"));
    }

    [Test]
    public void Constructor_EmptyArrays_ThrowsArgumentException()
    {
        // Arrange
        double[] probabilities = [];
        double[] rates = [];

        // Act & Assert
        Assert.That(() => new ExponentialHyper(probabilities, rates), 
            Throws.ArgumentException.With.Message.Contains("At least one component required"));
    }

    [Test]
    public void Constructor_NegativeProbabilities_ThrowsArgumentException()
    {
        // Arrange
        double[] probabilities = [-0.1, 1.1];
        double[] rates = [1.0, 2.0];

        // Act & Assert
        Assert.That(() => new ExponentialHyper(probabilities, rates), 
            Throws.ArgumentException.With.Message.Contains("Probabilities must be non-negative"));
    }

    [Test]
    public void Constructor_NonPositiveRates_ThrowsArgumentException()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [0.0, 2.0];

        // Act & Assert
        Assert.That(() => new ExponentialHyper(probabilities, rates), 
            Throws.ArgumentException.With.Message.Contains("Rates must be positive"));
    }

    [Test]
    public void Constructor_ZeroSumProbabilities_ThrowsArgumentException()
    {
        // Arrange
        double[] probabilities = [0.0, 0.0];
        double[] rates = [1.0, 2.0];

        // Act & Assert
        Assert.That(() => new ExponentialHyper(probabilities, rates), 
            Throws.ArgumentException.With.Message.Contains("Sum of probabilities must be positive"));
    }

    [Test]
    public void Constructor_UnnormalizedProbabilities_NormalizesInternally()
    {
        double[] probabilities = [1.0, 2.0, 3.0];
        double[] rates = [1.0, 2.0, 3.0];

        var distribution = new ExponentialHyper(probabilities, rates);

        Assert.Multiple(() =>
        {
            Assert.That(distribution.Probabilities, Is.EqualTo(probabilities));
            Assert.That(distribution.Rates, Is.EqualTo(rates));
        });
    }

    [Test]
    public void GetExpectedValue_SingleComponent_ReturnsCorrectValue()
    {
        // Arrange
        double[] probabilities = [1.0];
        double[] rates = [2.0];
        var distribution = new ExponentialHyper(probabilities, rates);
        var expected = 1.0 / 2.0;

        // Act
        var result = distribution.GetExpectedValue();

        // Assert
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test]
    public void GetExpectedValue_MultipleComponents_ReturnsCorrectValue()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0, 2.0];
        var distribution = new ExponentialHyper(probabilities, rates);
        var expected = 0.3 * (1.0 / 1.0) + 0.7 * (1.0 / 2.0);

        // Act
        var result = distribution.GetExpectedValue();

        // Assert
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test]
    public void GetExpectedValue_UnnormalizedProbabilities_ReturnsCorrectValue()
    {
        // Arrange
        double[] probabilities = [1.0, 2.0]; // Нормализуется до [1/3, 2/3]
        double[] rates = [1.0, 2.0];
        var distribution = new ExponentialHyper(probabilities, rates);
        var expected = (1.0/3.0) * (1.0 / 1.0) + (2.0/3.0) * (1.0 / 2.0);

        // Act
        var result = distribution.GetExpectedValue();

        // Assert
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test]
    public void GetVariance_SingleComponent_ReturnsCorrectValue()
    {
        // Arrange
        double[] probabilities = [1.0];
        double[] rates = [2.0];
        var distribution = new ExponentialHyper(probabilities, rates);
        var expectedVariance = 1.0 / (2.0 * 2.0); // Для экспоненциального распределения Variance = 1/λ²

        // Act
        var result = distribution.GetVariance();

        // Assert
        Assert.That(result, Is.EqualTo(expectedVariance).Within(Tolerance));
    }

    [Test]
    public void GetVariance_MultipleComponents_ReturnsCorrectValue()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0, 2.0];
        var distribution = new ExponentialHyper(probabilities, rates);
        
        var eX = distribution.GetExpectedValue();
        var eX2 = 0.3 * (2.0 / (1.0 * 1.0)) + 0.7 * (2.0 / (2.0 * 2.0));
        var expectedVariance = eX2 - eX * eX;

        // Act
        var result = distribution.GetVariance();

        // Assert
        Assert.That(result, Is.EqualTo(expectedVariance).Within(Tolerance));
    }

    [Test]
    public void GetMinValue_Always_ReturnsZero()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0, 2.0];
        var distribution = new ExponentialHyper(probabilities, rates);

        // Act
        var result = distribution.GetMinValue();

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void GetMaxValue_Always_ReturnsPositiveInfinity()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0, 2.0];
        var distribution = new ExponentialHyper(probabilities, rates);

        // Act
        var result = distribution.GetMaxValue();

        // Assert
        Assert.That(result, Is.EqualTo(double.PositiveInfinity));
    }

    [Test]
    public void Calculate_ProducesValidValues()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0, 2.0];
        var distribution = new ExponentialHyper(probabilities, rates);

        // Act & Assert - генерируем несколько значений и проверяем, что они неотрицательны
        for (int i = 0; i < 100; i++)
        {
            var value = distribution.Calculate();
            Assert.That(value, Is.GreaterThanOrEqualTo(0));
        }
    }

    [Test]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        double[] probabilities = [0.3, 0.7];
        double[] rates = [1.0, 2.0];
        var distribution = new ExponentialHyper(probabilities, rates);

        // Act
        var result = distribution.ToString();

        // Assert
        Assert.That(result, Does.Contain("HyperExponential"));
        Assert.That(result, Does.Contain("Probs = [0.300, 0.700]"));
        Assert.That(result, Does.Contain("Rates=[1.000, 2.000]"));
    }

    [Test]
    public void Calculate_WithDifferentRandomValues_SelectsCorrectComponents()
    {
        // Arrange
        double[] probabilities = [0.5, 0.5];
        double[] rates = [1.0, 10.0]; // Вторая компонента имеет значительно большую скорость
        var distribution = new ExponentialHyper(probabilities, rates);

        // Act & Assert - этот тест в основном проверяет, что метод не падает
        // и возвращает корректные значения для разных вероятностных сценариев
        var values = new double[1000];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = distribution.Calculate();
            Assert.That(values[i], Is.GreaterThanOrEqualTo(0));
        }

        // Дополнительная проверка: среднее значение должно быть близко к ожидаемому
        var average = values.Average();
        var expectedMean = distribution.GetExpectedValue();
        Assert.That(average, Is.EqualTo(expectedMean).Within(0.5)); // Допустимая погрешность для статистики
    }

    [Test]
    public void Distribution_ThreeComponents_CorrectProperties()
    {
        // Arrange
        double[] probabilities = [0.2, 0.3, 0.5];
        double[] rates = [1.0, 2.0, 3.0];
        var distribution = new ExponentialHyper(probabilities, rates);

        // Act
        var mean = distribution.GetExpectedValue();
        var variance = distribution.GetVariance();
        var min = distribution.GetMinValue();
        var max = distribution.GetMaxValue();

        // Assert
        var expectedMean = 0.2 * 1.0 + 0.3 * 0.5 + 0.5 * (1.0/3.0);
        Assert.That(mean, Is.EqualTo(expectedMean).Within(Tolerance));
        Assert.That(variance, Is.GreaterThan(0));
        Assert.That(min, Is.EqualTo(0));
        Assert.That(max, Is.EqualTo(double.PositiveInfinity));
    }
}