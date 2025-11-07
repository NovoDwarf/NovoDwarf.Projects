using Mathematics.Models.Distributions.Basic;

namespace Mathematics.Tests.Models.Distributions.Basic;

[TestFixture]
public class DeterministicTests
{
	[Test]
	public void Calculate_WithDeterministicOne_ReturnsOne([Random(double.MinValue, double.MaxValue, 10)] double constant)
	{
		var distribution = new Deterministic(constant);
		
		var result = distribution.Calculate();
		
		Assert.That(result, Is.EqualTo(constant));
	}
}