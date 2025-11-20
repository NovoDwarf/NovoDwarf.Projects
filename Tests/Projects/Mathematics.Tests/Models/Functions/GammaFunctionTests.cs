using Mathematics.Core.Functions.Models;

namespace Mathematics.Tests.Models.Functions;

public class GammaFunctionTests
{
	[Test]
	[TestCase(1, 1.0)]
	[TestCase(2, 1.0)]
	[TestCase(3, 2.0)]
	[TestCase(4, 6.0)]
	[TestCase(5, 24.0)]
	public void Gamma_WithIntegerValues_ReturnsFactorial(int x, double expected)
	{
		Assert.That(GammaFunction.Calculate(x), Is.EqualTo(expected).Within(1e-10));
	}

	[Test]
	public void Gamma_WithHalfInteger_ReturnsCorrectValue()
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(GammaFunction.Calculate(0.5), Is.EqualTo(Math.Sqrt(Math.PI)).Within(1e-10));
			Assert.That(GammaFunction.Calculate(1.5), Is.EqualTo(0.5 * Math.Sqrt(Math.PI)).Within(1e-10));
			Assert.That(GammaFunction.Calculate(2.5), Is.EqualTo(1.5 * 0.5 * Math.Sqrt(Math.PI)).Within(1e-10));
		}
	}

	[Test]
	public void Gamma_WithLargeValue_ReturnsFiniteResult()
	{
		var result = GammaFunction.Calculate(100);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.GreaterThan(0));
			Assert.That(double.IsInfinity(result), Is.False);
		}
	}
}