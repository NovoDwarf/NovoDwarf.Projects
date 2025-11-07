using Mathematics.Interfaces;

namespace Mathematics.Models.Base;

/// <summary>
/// Base class for all distributions.
/// </summary>
public abstract class DistributionBase : IDistribution
{
    /// <summary>
    /// Generates a random value.
    /// </summary>
    /// <returns>Random value.</returns>
    public abstract double Calculate();
    
    /// <summary>
    /// Returns the expected value of the distribution.
    /// </summary>
    public virtual double GetExpectedValue() => double.NaN;

    /// <summary>
    /// Returns the variance of the distribution.
    /// </summary>
    public virtual double GetVariance() => double.NaN;

    /// <summary>
    /// Returns the standard deviation of the distribution.
    /// </summary>
    public virtual double GetStandardDeviation() => Math.Sqrt(GetVariance());

    /// <summary>
    /// Returns the minimum value.
    /// </summary>
    public virtual double GetMinValue() => double.NegativeInfinity;

    /// <summary>
    /// Returns the maximum value.
    /// </summary>
    public virtual double GetMaxValue() => double.PositiveInfinity;

    /// <summary>
    /// Checks if the value is valid for this distribution.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>True if the value is valid.</returns>
    public virtual bool IsValidValue(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value) && 
               value >= GetMinValue() && value <= GetMaxValue();
    }

    /// <summary>
    /// Generates a random value and validates it.
    /// </summary>
    /// <returns>Validated random value.</returns>
    public double CalculateValidated()
    {
        var value = Calculate();
        
        return !IsValidValue(value) 
            ? throw new InvalidOperationException($"Generated value [{value}] is not valid for this distribution.") 
            : value;
    }
}




