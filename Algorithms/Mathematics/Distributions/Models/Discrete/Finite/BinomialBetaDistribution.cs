using Mathematics.Distributions.Base;
using Mathematics.Distributions.Models.Continuous.Bounded;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Discrete.Finite;

public class BinomialBetaDistribution : DistributionBase
{
    private readonly double _alpha;
    private readonly double _beta;
    private readonly int _trials;
    
    private readonly BetaDistribution _betaDist;

    public BinomialBetaDistribution(double alpha, double beta, int trials)
    {
        if (alpha <= 0)
            throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must be positive.");
        if (beta <= 0)
            throw new ArgumentOutOfRangeException(nameof(beta), "Beta must be positive.");
        if (trials < 0)
            throw new ArgumentOutOfRangeException(nameof(trials), "Number of trials must be non-negative.");

        _alpha = alpha;
        _beta = beta;
        _trials = trials;
        _betaDist = new BetaDistribution(alpha, beta, 1);
    }

    public double Alpha => _alpha;
    public double Beta => _beta;
    public int Trials => _trials;

    public override double Calculate()
    {
        var p = _betaDist.Calculate();
        
        return SampleBinomial(_trials, p);
    }

    public override double GetExpectedValue() => _trials * (_alpha / (_alpha + _beta));

    public override double GetVariance()
    {
        var alphaBeta = _alpha + _beta;
       
        return _trials * _alpha * _beta * (alphaBeta + _trials) / (alphaBeta * alphaBeta * (alphaBeta + 1));
    }

    public override double GetMinValue() => 0.0;

    public override double GetMaxValue() => _trials;

    private int SampleBinomial(int n, double p)
    {
        if (n == 0 || p == 0) 
            return 0;
        
        if (p == 1) 
            return n;

        var successes = 0;
        
        for (var i = 0; i < n; i++)
        {
            if (RandomUtils.NextDouble() < p)
                successes++;
        }
        return successes;
    }
}