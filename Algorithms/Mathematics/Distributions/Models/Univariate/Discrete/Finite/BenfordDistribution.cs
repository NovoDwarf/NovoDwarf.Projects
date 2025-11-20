using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Discrete.Finite;

public partial class BenfordDistribution : Distribution
{
    public override double Calculate()
    {
        var randomValue = RandomUtils.NextDouble();
        var cumulativeProbability = 0.0;

        for (var digit = 1; digit <= 9; digit++)
        {
            cumulativeProbability += GetProbabilityForDigit(digit);
            
            if (randomValue <= cumulativeProbability)
                return digit;
        }

        return 9;
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
        var expectedValue = 0.0;
       
        for (var digit = 1; digit <= 9; digit++) 
            expectedValue += digit * GetProbabilityForDigit(digit);
        
        return expectedValue;
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
        var expectedValue = GetExpectedValue();
        var expectedSquares = 0.0;

        for (var digit = 1; digit <= 9; digit++) 
            expectedSquares += digit * digit * GetProbabilityForDigit(digit);

        return expectedSquares - expectedValue * expectedValue;
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

    public override double GetMinValue() => 1.0;

    public override double GetMaxValue() => 9.0;

    private static double GetProbabilityForDigit(int digit)
    {
        if (digit is < 1 or > 9)
            throw new ArgumentOutOfRangeException(nameof(digit), "Digit must be between 1 and 9.");

        return Math.Log10(1.0 + 1.0 / digit);
    }
}