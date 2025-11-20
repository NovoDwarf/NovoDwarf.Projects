using Mathematics.Distributions.Base;
using Mathematics.Transforms.Models;

namespace Mathematics.Distributions.Models.Univariate.Continuous.RealLine;

public partial class StudentsTDistribution : Distribution
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

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue()
	{
		return DegreesOfFreedom > 1 ? 0 : double.NaN;
	}

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

	public override double GetVariance()
	{
		return DegreesOfFreedom > 2 ? DegreesOfFreedom / (double)(DegreesOfFreedom - 2) : double.NaN;
	}

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