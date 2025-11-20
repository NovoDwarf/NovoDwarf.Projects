using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Transforms.Models;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class LevyDistribution : Distribution
{
	private readonly double _location;
	private readonly double _scale;
	
	public LevyDistribution(double location, double scale)
	{
		_location = location;
		_scale = scale;
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
		var (z, _) = BoxMullerTransform.Polar();

		return _location + _scale / (z * z);
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