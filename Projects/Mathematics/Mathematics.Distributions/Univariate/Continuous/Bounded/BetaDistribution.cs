using Mathematics.Core.Base;
using Mathematics.Distributions.Univariate.Continuous.Semibounded;
using Mathematics.Functions;

namespace Mathematics.Distributions.Univariate.Continuous.Bounded;

public partial class BetaDistribution : Distribution
{
	private readonly GammaDistribution _gammaDistributionAlpha;
	private readonly GammaDistribution _gammaDistributionBeta;
	
	private readonly double _alpha;
	private readonly double _beta;
	private readonly double _scale;
	
	public BetaDistribution(double alpha = 1, double beta = 1, double scale = 1)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(alpha);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(beta);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(scale);

		_alpha = alpha;
		_beta = beta;
		_scale = scale;

		_gammaDistributionAlpha = new GammaDistribution(_alpha, _scale);
		_gammaDistributionBeta = new GammaDistribution(_beta, _scale);
	}
	
	public override double Expected => _alpha / (_alpha + _beta);

	public override double Mean => _alpha / (_alpha + _beta);

	public override double Median => _alpha == _beta ? 0.5 : (_alpha - 1.0 / 3.0) / (_alpha + _beta - 2.0 / 3.0);
	
	public override double Mode => GetMode();

	public override double Variance => _alpha * _beta / ((_alpha + _beta) * (_alpha + _beta) * (_alpha + _beta + 1));

	public override double Skewness => GetSkewness();

	public override double Kurtosis => GetKurtosis();

	public override double StandardDeviation => Math.Sqrt(Variance);

	public override double Minimum => 0;

	public override double Maximum => 1;
	
	public override double Distribute()
	{
		var y1 = _gammaDistributionAlpha.Distribute();
		var y2 = _gammaDistributionBeta.Distribute();

		return y1 / (y1 + y2);
	}

	public override double Quantile(double p)
	{
		return p; // TODO: impelement this
	}

	public override double ProbabilityDensity(double x)
	{
		if (x is < 0 or > 1)
			return 0;

		var betaFunction = GammaFunction.Calculate(_alpha) * GammaFunction.Calculate(_beta) / GammaFunction.Calculate(_alpha + _beta);
		
		return Math.Pow(x, _alpha - 1) * Math.Pow(1 - x, _beta - 1) / betaFunction;
	}

	public override double CumulativeDistribution(double x)
	{
		return x switch
		{
			<= 0 => 0,
			>= 1 => 1,
			_ => RegularizedIncompleteBetaFunction.Calculate(x, _alpha, _beta)
		};
	}
	
	public override string ToString() => $"Beta Distribution [Alpha = {_alpha}, Beta = {_beta}, Scale = {_scale}]";
	
	private double GetSkewness()
	{
		var numerator = 2 * (_beta - _alpha) * Math.Sqrt(_alpha + _beta + 1);
		var denominator = (_alpha + _beta + 2) * Math.Sqrt(_alpha * _beta);
		
		return numerator / denominator;
	}
	
	private double GetKurtosis()
	{
		var sum = _alpha + _beta;
		var product = _alpha * _beta;
		var numerator = 6 * ((_alpha - _beta) * (_alpha - _beta) * (sum + 1) - product * (sum + 2));
		var denominator = product * (sum + 2) * (sum + 3);
		
		return numerator / denominator;
	}

	private double GetMode()
	{
		return _alpha switch
		{
			> 1 when _beta > 1 => (_alpha - 1) / (_alpha + _beta - 2),
			1 when _beta == 1 => 0.5,
			< 1 when _beta < 1 => double.NaN,
			<= 1 when _beta > 1 => 0,
			_ => 1
		};
	}
}