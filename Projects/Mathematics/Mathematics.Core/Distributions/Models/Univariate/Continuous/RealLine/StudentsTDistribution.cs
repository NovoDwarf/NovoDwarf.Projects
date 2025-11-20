using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Transforms.Models;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.RealLine;

public partial class StudentsTDistribution : Distribution
{
	private readonly int _degreesOfFreedom;
	
	public StudentsTDistribution(int degreesOfFreedom)
	{
		_degreesOfFreedom = degreesOfFreedom;
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

		var v = 0.0;

		for (var i = 0; i < _degreesOfFreedom; i++)
		{
			var (zi, _) = BoxMullerTransform.Polar();
			v += zi * zi;
		}

		return z / Math.Sqrt(v / _degreesOfFreedom);
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