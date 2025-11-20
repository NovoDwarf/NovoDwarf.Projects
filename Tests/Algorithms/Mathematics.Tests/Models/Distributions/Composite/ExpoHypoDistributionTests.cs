using Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

namespace Mathematics.Tests.Models.Distributions.Composite;

[TestFixture]
public class ExpoHypoDistributionTests
{
	[Test]
	public void Calculate_WithTwoStages_MatchesErlangForEqualRates()
	{
		var rates = new[] { 2.0, 2.0 };
		var hypo = new ExpoHypoDistribution(rates);
		var erlang = new ErlangDistribution(2, 2.0);

		var hypoMean = hypo.GetExpectedValue();
		var erlangMean = erlang.GetExpectedValue();

		Assert.That(hypoMean, Is.EqualTo(erlangMean).Within(1e-10));
	}

	[Test]
	public void GetVariance_WithDifferentRates_CorrectlyCalculated()
	{
		var rates = new[] { 1.0, 2.0 };
		var hypo = new ExpoHypoDistribution(rates);

		var variance = hypo.GetVariance();

		Assert.That(variance, Is.EqualTo(1.25).Within(1e-10));
	}
}