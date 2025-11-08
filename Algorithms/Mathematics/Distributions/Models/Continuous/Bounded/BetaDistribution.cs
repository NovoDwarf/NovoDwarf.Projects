using Mathematics.Distributions.Base;
using Mathematics.Distributions.Models.Continuous.SemiInfinite;

namespace Mathematics.Distributions.Models.Continuous.Bounded;

public class BetaDistribution : DistributionBase
{
	private readonly GammaDistribution _gammaDistributionAlpha;
	private readonly GammaDistribution _gammaDistributionBeta;

	public BetaDistribution(double alphaParam, double betaParam, double scaleParam)
	{
		AlphaParam = alphaParam;
		BetaParam = betaParam;
		ScaleParam = scaleParam;

		_gammaDistributionAlpha = new GammaDistribution(AlphaParam, ScaleParam);
		_gammaDistributionBeta = new GammaDistribution(BetaParam, ScaleParam);
	}

	public double AlphaParam { get; }
	public double BetaParam { get; }
	public double ScaleParam { get; }

	public override double Calculate()
	{
		var y1 = _gammaDistributionAlpha.Calculate();
		var y2 = _gammaDistributionBeta.Calculate();

		return y1 / (y1 + y2);
	}

	public override double GetExpectedValue()
	{
		return AlphaParam / (AlphaParam + BetaParam);
	}

	public override double GetVariance()
	{
		return AlphaParam * BetaParam /
		       ((AlphaParam + BetaParam) * (AlphaParam + BetaParam) * (AlphaParam + BetaParam + 1));
	}

	public override double GetMinValue()
	{
		return 0;
	}

	public override double GetMaxValue()
	{
		return 1;
	}

	public override string ToString()
	{
		return $"Beta [α = {AlphaParam:F3}, β = {BetaParam:F3}, Scale = {ScaleParam:F3}]";
	}
}