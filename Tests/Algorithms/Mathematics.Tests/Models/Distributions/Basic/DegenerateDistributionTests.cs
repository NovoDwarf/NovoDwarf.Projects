using Mathematics.Distributions.Models.Degenerate;

namespace Mathematics.Tests.Models.Distributions.Basic;

[TestFixture]
public class DegenerateDistributionTests
{
	[Test]
	public void Calculate_WithDeterministicOne_ReturnsOne([Random(double.MinValue, double.MaxValue, 10)] double constant)
	{
		var distribution = new DegenerateDistribution(constant);

		var result = distribution.Calculate();

		Assert.That(result, Is.EqualTo(constant));
	}
}