using Mathematics.Core.Enums;

namespace Mathematics.Core.Base;

/// <summary>
///     Base class for all distributions.
/// </summary>
public abstract class Distribution : Entity
{
	public virtual string Name => "_name";
	public virtual string Description => "_desc";

	public override string Category => "distribution_category";
	
	public virtual DistributionCategory DistributionCategory => DistributionCategory.None;
	public virtual DistributionSubCategory DistributionSubCategory => DistributionSubCategory.None;

	/// <summary>
	/// Returns the expected value of the distribution
	/// </summary>
	public abstract double Expected { get; }

	/// <summary>
	/// Returns the mean of the distribution
	/// </summary>
	public abstract double Mean { get; } 

	/// <summary>
	/// Returns the median of the distribution
	/// </summary>
	public abstract double Median { get; }

	/// <summary>
	/// Returns the mode of the distribution
	/// </summary>
	public abstract double Mode { get; }

	/// <summary>
	/// Returns the variance of the distribution
	/// </summary>
	public abstract double Variance { get; }

	/// <summary>
	/// Returns the skewness of the distribution
	/// </summary>
	public abstract double Skewness { get; }

	/// <summary>
	/// Returns the kurtosis of the distribution
	/// </summary>
	public abstract double Kurtosis { get; }

	/// <summary>
	/// Returns the standard deviation of the distribution
	/// </summary>
	public abstract double StandardDeviation { get; }

	/// <summary>
	/// Returns the minimum value
	/// </summary>
	public abstract double Minimum { get; }

	/// <summary>
	/// Returns the maximum value
	/// </summary>
	public abstract double Maximum { get; }
	
	/// <summary>
	/// Returns a random value from the distribution
	/// </summary>
	/// <returns>Random value</returns>
	public abstract double Distribute();

	/// <summary>
	/// Returns the quantile of the distribution
	/// </summary>
	/// <param name="p"></param>
	/// <returns></returns>
	public abstract double Quantile(double p);
	
	/// <summary>
	/// Returns the probability density of the distribution
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public abstract double ProbabilityDensity(double x);

	/// <summary>
	/// Returns the cumulative distribution of the distribution
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public abstract double CumulativeDistribution(double x);
}