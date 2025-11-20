using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.Bounded;

/// <summary>
///     Represents the beta distribution.
/// </summary>
public partial class BetaDistribution : Distribution
{
	private readonly GammaDistribution _gammaDistributionAlpha;
	private readonly GammaDistribution _gammaDistributionBeta;
	
	private readonly double _alpha;
	private readonly double _beta;
	private readonly double _scale;
	
	public BetaDistribution(double alpha, double beta, double scale)
	{
		_alpha = alpha;
		_beta = beta;
		_scale = scale;

		_gammaDistributionAlpha = new GammaDistribution(_alpha, _scale);
		_gammaDistributionBeta = new GammaDistribution(_beta, _scale);
	}
	
	public override double Expected => _alpha / (_alpha + _beta);

	public override double Mean => throw new NotImplementedException();

	public override double Median => throw new NotImplementedException();

	public override double Mode => throw new NotImplementedException();

	public override double Variance => _alpha * _beta / ((_alpha + _beta) * (_alpha + _beta) * (_alpha + _beta + 1));

	public override double Skewness => throw new NotImplementedException();

	public override double Kurtosis => throw new NotImplementedException();

	public override double StandardDeviation => throw new NotImplementedException();

	public override double Minimum => 0;

	public override double Maximum => 1;
	
	public override double Distribute()
	{
		var y1 = _gammaDistributionAlpha.Distribute();
		var y2 = _gammaDistributionBeta.Distribute();

		return y1 / (y1 + y2);
	}

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}
}