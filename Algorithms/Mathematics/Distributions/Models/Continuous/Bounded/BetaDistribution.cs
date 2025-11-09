using Mathematics.Distributions.Base;
using Mathematics.Distributions.Models.Continuous.SemiInfinite;

namespace Mathematics.Distributions.Models.Continuous.Bounded;

/// <summary>
/// Represents the beta distribution.
/// </summary>
public class BetaDistribution : DistributionBase
{
	private readonly GammaDistribution _gammaDistributionAlpha;
	private readonly GammaDistribution _gammaDistributionBeta;

	public BetaDistribution(double alpha, double beta, double scale)
	{
		Alpha = alpha;
		Beta = beta;
		Scale = scale;

		_gammaDistributionAlpha = new GammaDistribution(Alpha, Scale);
		_gammaDistributionBeta = new GammaDistribution(Beta, Scale);
	}

	public double Alpha { get; }
	public double Beta { get; }
	public double Scale { get; }

	public override double Calculate()
	{
		var y1 = _gammaDistributionAlpha.Calculate();
		var y2 = _gammaDistributionBeta.Calculate();

		return y1 / (y1 + y2);
	}

	public override double GetExpectedValue() => Alpha / (Alpha + Beta);

	public override double GetVariance() => Alpha * Beta / ((Alpha + Beta) * (Alpha + Beta) * (Alpha + Beta + 1));

	public override double GetMinValue() => 0;

	public override double GetMaxValue() => 1;

	public override string ToString() => $"Beta [α = {Alpha:F3}, β = {Beta:F3}, Scale = {Scale:F3}]";
}