using Mathematics.Utilities.Formulas;

namespace Mathematics.Tests.Utilities.Formulas;

public class GammaUtilsTests
{
	[Test]
	public void Gamma_WithIntegerValues_ReturnsFactorial()
	{
        Assert.Multiple(() =>
        {
            Assert.That(GammaUtils.Gamma(1), Is.EqualTo(1.0).Within(1e-10));
            Assert.That(GammaUtils.Gamma(2), Is.EqualTo(1.0).Within(1e-10));
            Assert.That(GammaUtils.Gamma(3), Is.EqualTo(2.0).Within(1e-10));
            Assert.That(GammaUtils.Gamma(4), Is.EqualTo(6.0).Within(1e-10));
            Assert.That(GammaUtils.Gamma(5), Is.EqualTo(24.0).Within(1e-10));
        });
    }
    
	[Test]
	public void Gamma_WithHalfInteger_ReturnsCorrectValue()
	{
        Assert.Multiple(() =>
        {
            Assert.That(GammaUtils.Gamma(0.5), Is.EqualTo(Math.Sqrt(Math.PI)).Within(1e-10));
            Assert.That(GammaUtils.Gamma(1.5), Is.EqualTo(0.5 * Math.Sqrt(Math.PI)).Within(1e-10));
            Assert.That(GammaUtils.Gamma(2.5), Is.EqualTo(1.5 * 0.5 * Math.Sqrt(Math.PI)).Within(1e-10));
        });
    }
    
	[Test]
	public void Gamma_WithLargeValue_ReturnsFiniteResult()
	{
		var result = GammaUtils.Gamma(100);
        
		Assert.Multiple(() =>
        {
            Assert.That(result, Is.GreaterThan(0));
            Assert.That(double.IsInfinity(result), Is.False);
        });
    }
}