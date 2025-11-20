using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.RealLine;

public partial class CauchyDistribution : Distribution
{
	private readonly double _location;
	private readonly double _scale;
	
	public CauchyDistribution(double location, double scale)
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
		var u = RandomUtils.NextDouble();

		return _location + _scale * Math.Tan(Math.PI * (u - 0.5));
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