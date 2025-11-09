using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;
using Utilities.Extensions;

namespace Mathematics.Distributions.Models.Discrete.Finite;

public class BinomialPoissonDistribution : DistributionBase
{
    private readonly double[] _successProbabilities;
    
    public BinomialPoissonDistribution(double[] successProbabilities)
    {
        ArgumentException.ThrowIfNullOrEmpty(successProbabilities);
        ArgumentOutOfRangeException.ThrowIfOutOfRange(successProbabilities, 0, 1);

        _successProbabilities = (double[])successProbabilities.Clone();
    }

    public IReadOnlyList<double> SuccessProbabilities => _successProbabilities;
    public int Trials => _successProbabilities.Length;

    public override double Calculate() => SamplePoissonBinomial(_successProbabilities);

    public override double GetExpectedValue() => _successProbabilities.Sum();

    public override double GetVariance() => _successProbabilities.Sum(p => p * (1 - p));

    public override double GetMinValue() => 0.0;

    public override double GetMaxValue() => _successProbabilities.Length;

    private int SamplePoissonBinomial(double[] probabilities)
    {
        if (probabilities.Length == 0)
            return 0;

        return probabilities.Count(p => RandomUtils.NextDouble() < p);
    }
}