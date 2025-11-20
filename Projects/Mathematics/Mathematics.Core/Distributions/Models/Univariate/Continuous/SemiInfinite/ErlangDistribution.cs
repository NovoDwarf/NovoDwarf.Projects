using Mathematics.Core.Distributions.Base;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class ErlangDistribution : Distribution
{
	private readonly int _shape;
	private readonly double _rate;
	
	public ErlangDistribution(int shape, double rate)
	{
		if (shape < 1)
			throw new ArgumentException("Shape must be greater than 0.", nameof(shape));

		if (rate <= 0)
			throw new ArgumentException("Rate must be greater than 0.", nameof(rate));

		_shape = shape;
		_rate = rate;
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
		var sum = 0.0;
		var exp = new ExpoDistribution(_rate);

		for (var i = 0; i < _shape; i++)
			sum += exp.Distribute();

		return sum;
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