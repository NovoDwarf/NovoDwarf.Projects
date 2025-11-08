using Range = Structures.Models.Base.Range;

namespace Structures.Models.Constants;

public static class ParameterRanges
{
	public static readonly Range Positive = new(0, double.PositiveInfinity, false);
	public static readonly Range NonNegative = new(0, double.PositiveInfinity);
	public static readonly Range Finite = new(double.NegativeInfinity, double.PositiveInfinity, false);
	public static readonly Range Probability = new(0, 1, true, true);
	public static readonly Range DegreesOfFreedom = new(1, int.MaxValue);
}