using Data.Attributes;
using Mathematics.Distributions.Interfaces;

namespace Mathematics.Distributions.Base;

/// <summary>
/// Base class for all distributions.
/// </summary>
public abstract class DistributionBase : IDistribution<double>
{
	/// <summary>
	/// Generates a random value
	/// </summary>
	/// <returns>Random value</returns>
	[MustOverride]
	public abstract double Calculate();

	/// <summary>
	/// Returns the expected value of the distribution
	/// </summary>
	[MustOverride]
	public virtual double GetExpectedValue() => double.NaN;

	/// <summary>
	///  Returns the variance of the distribution
	/// </summary>
	[MustOverride]
	public virtual double GetVariance() => double.NaN;

	/// <summary>
	///  Returns the standard deviation of the distribution
	/// </summary>
	[MustOverride]
	public virtual double GetStandardDeviation() => Math.Sqrt(GetVariance());

	/// <summary>
	/// Returns the minimum value
	/// </summary>
	[MustOverride]
	public virtual double GetMinValue() => double.NegativeInfinity;

	/// <summary>
	/// Returns the maximum value
	/// </summary>
	[MustOverride]
	public virtual double GetMaxValue() => double.PositiveInfinity;

	/// <inheritdoc cref="object.ToString()"/>
	[MustOverride]
	public override string ToString() => "Distribution Base";
}