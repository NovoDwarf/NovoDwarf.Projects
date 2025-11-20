using Data.Attributes;
using Mathematics.Base;
using Mathematics.Distributions.Enums;
using Mathematics.Distributions.Interfaces;

namespace Mathematics.Distributions.Base;

/// <summary>
/// Base class for all distributions.
/// </summary>
public abstract partial class Distribution : Entity, IDistribution<double>
{
	public virtual string Name => "_name";
	public virtual string Description => "_desc";

	public virtual DistributionCategory Category => DistributionCategory.None;
	public virtual DistributionSubCategory SubCategory => DistributionSubCategory.None;

	/// <summary>
	/// Generates a random value
	/// </summary>
	/// <returns>Random value</returns>
	public abstract double Calculate();

	public abstract double GetProbabilityDensity(double x);
	
	public abstract double GetCumulativeDistribution(double x);
	
	/// <summary>
	/// Returns the expected value of the distribution
	/// </summary>
	public abstract double GetExpectedValue();
	
	public abstract double GetMean();
	
	public abstract double GetMedian();
	
	public abstract double GetMode();

	/// <summary>
	///  Returns the variance of the distribution
	/// </summary>
	public abstract double GetVariance();

	public abstract double GetSkewness();
	
	public abstract double GetKurtosis();
	
	/// <summary>
	///  Returns the standard deviation of the distribution
	/// </summary>
	public abstract double GetStandardDeviation();

	/// <summary>
	/// Returns the minimum value
	/// </summary>
	public abstract double GetMinValue();

	/// <summary>
	/// Returns the maximum value
	/// </summary>
	public abstract double GetMaxValue();
}