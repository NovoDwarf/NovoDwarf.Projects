using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;
using Mathematics.Numerical.Simple;
using Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Bounded;

[Categories("Distributions", "Univariate", "Continious", "Bounded")]
public partial class BetaDistribution : Distribution
{
	public override double Expected => Alpha / (Alpha + Beta);

	public override double Mean => Alpha / (Alpha + Beta);

	public override double Median => DoubleUtils.Approximately(Alpha, Beta) ? 0.5 : (Alpha - 1.0 / 3.0) / (Alpha + Beta - 2.0 / 3.0);
	
	public override double Mode => GetMode();

	public override double Variance => Alpha * Beta / ((Alpha + Beta) * (Alpha + Beta) * (Alpha + Beta + 1));

	public override double Skewness => GetSkewness();

	public override double Kurtosis => GetKurtosis();

	public override double StandardDeviation => Math.Sqrt(Variance);

	public override double Minimum => 0;

	public override double Maximum => 1;
	
	[Range(1, double.PositiveInfinity)]
	[EntityParameter(typeof(double), nameof(Alpha))]
	public double Alpha { get; private set; } = 1;

	[Range(1, double.PositiveInfinity)]
	[EntityParameter(typeof(double), nameof(Beta))]
	public double Beta { get; private set; } = 1;

	[Range(1, double.PositiveInfinity)]
	[EntityParameter(typeof(double), nameof(Scale))]
	public double Scale { get; private set; } = 1;
	
	private GammaDistribution _gammaDistributionAlpha = new();
	private GammaDistribution _gammaDistributionBeta = new();
	
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

		var betaFunction = GammaFunction.Calculate(Alpha) * GammaFunction.Calculate(Beta) / GammaFunction.Calculate(Alpha + Beta);
		
		return Math.Pow(x, Alpha - 1) * Math.Pow(1 - x, Beta - 1) / betaFunction;
	}

	public override double CumulativeDistribution(double x)
	{
		return x switch
		{
			<= 0 => 0,
			>= 1 => 1,
			_ => BetaRegularizedIncompleteFunction.Calculate(x, Alpha, Beta)
		};
	}
	
	public override string ToString() => $"Beta Distribution [Alpha = {Alpha}, Beta = {Beta}, Scale = {Scale}]";
	
	protected override void Validate()
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Alpha);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Beta);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Scale);
		
		_gammaDistributionAlpha.Set(Alpha, Scale);
		_gammaDistributionBeta.Set(Beta, Scale);
	}
	
	private double GetSkewness()
	{
		var numerator = 2 * (Beta - Alpha) * Math.Sqrt(Alpha + Beta + 1);
		var denominator = (Alpha + Beta + 2) * Math.Sqrt(Alpha * Beta);
		
		return numerator / denominator;
	}
	
	private double GetKurtosis()
	{
		var sum = Alpha + Beta;
		var product = Alpha * Beta;
		var numerator = 6 * ((Alpha - Beta) * (Alpha - Beta) * (sum + 1) - product * (sum + 2));
		var denominator = product * (sum + 2) * (sum + 3);
		
		return numerator / denominator;
	}

	private double GetMode()
	{
		return Alpha switch
		{
			> 1 when Beta > 1 => (Alpha - 1) / (Alpha + Beta - 2),
			1 when DoubleUtils.Approximately(Beta, 1) => 0.5,
			< 1 when Beta < 1 => double.NaN,
			<= 1 when Beta > 1 => 0,
			_ => 1
		};
	}
}