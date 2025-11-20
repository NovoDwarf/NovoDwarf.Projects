using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.RealLine;

public partial class NormalDistribution : Distribution
{
	private readonly double _mean;
	private readonly double _stdDev;
	
	public NormalDistribution(double mean, double stdDev)
	{
		_mean = mean;
		_stdDev = stdDev;
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
	
	public override double Distribute() => RandomUtils.NextNormal();
	
	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}
}