using Mathematics.Distributions.Base;
using Mathematics.Transforms.Models;

namespace Mathematics.Distributions.Models.Continuous.RealLine;

public class StudentsTDistribution : DistributionBase
{
	public StudentsTDistribution(int degreesOfFreedom)
	{
		DegreesOfFreedom = degreesOfFreedom;
	}

	public int DegreesOfFreedom { get; }

	public override double Calculate()
	{
		var (z, _) = BoxMullerTransform.Polar();

		var v = 0.0;

		for (var i = 0; i < DegreesOfFreedom; i++)
		{
			var (zi, _) = BoxMullerTransform.Polar();
			v += zi * zi;
		}

		return z / Math.Sqrt(v / DegreesOfFreedom);
	}

	public override double GetExpectedValue()
	{
		return DegreesOfFreedom > 1 ? 0 : double.NaN;
	}

	public override double GetVariance()
	{
		return DegreesOfFreedom > 2 ? DegreesOfFreedom / (double)(DegreesOfFreedom - 2) : double.NaN;
	}

	public override double GetMinValue()
	{
		return double.NegativeInfinity;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"StudentsT [Degrees of Freedom = {DegreesOfFreedom}]";
	}
}