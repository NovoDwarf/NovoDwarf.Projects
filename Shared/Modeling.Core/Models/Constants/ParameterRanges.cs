using Base_Range = Modeling.Core.Models.Base.Range;

namespace Modeling.Core.Models.Constants;

public static class ParameterRanges
{
	public static readonly Base_Range Positive = new(0, double.PositiveInfinity, false);
	public static readonly Base_Range NonNegative = new(0, double.PositiveInfinity);
	public static readonly Base_Range Finite = new(double.NegativeInfinity, double.PositiveInfinity, false);
	public static readonly Base_Range Probability = new(0, 1, true, true);
	public static readonly Base_Range DegreesOfFreedom = new(1, int.MaxValue);
}