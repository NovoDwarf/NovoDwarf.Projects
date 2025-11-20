using Mathematics.Distributions.Base;
using Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

namespace Mathematics.Distributions.Models.Univariate.Continuous.Bounded;

/// <summary>
/// Represents the beta distribution.
/// </summary>
public partial class BetaDistribution : Distribution
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

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue() => Alpha / (Alpha + Beta);
	
	public override double GetMean()
	{
		throw new NotImplementedException();
	}

	public override double GetMedian()
	{
		throw new NotImplementedException();
	}

	public override double GetMode()
	{
		throw new NotImplementedException();
	}

	public override double GetVariance() => Alpha * Beta / ((Alpha + Beta) * (Alpha + Beta) * (Alpha + Beta + 1));
	
	public override double GetSkewness()
	{
		throw new NotImplementedException();
	}

	public override double GetKurtosis()
	{
		throw new NotImplementedException();
	}

	public override double GetStandardDeviation()
	{
		throw new NotImplementedException();
	}

	public override double GetMinValue() => 0;

	public override double GetMaxValue() => 1;
}