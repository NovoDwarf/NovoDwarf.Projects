using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Functions.Models;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class WeibullDistribution : Distribution
{
	private readonly double _scale;
	private readonly double _shape;
	
	public WeibullDistribution(double scale, double shape)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0);
		ArgumentOutOfRangeException.ThrowIfLessThan(shape, 0);

		_scale = scale;
		_shape = shape;
	}
	
	public override double Expected { get; }
	public override double Mean { get; }
	public override double Median { get; }
	public override double Mode { get; }
	public override double Variance { get; }
	public override double Skewness { get; }
	public override double Kurtosis { get; }
	public override double StandardDeviation { get; }
	public override double Minimum { get; }
	public override double Maximum { get; }

	public override double Distribute()
	{
		var u = RandomUtils.NextDoubleSafe();

		return _scale * Math.Pow(-Math.Log(u), 1.0 / _shape);
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