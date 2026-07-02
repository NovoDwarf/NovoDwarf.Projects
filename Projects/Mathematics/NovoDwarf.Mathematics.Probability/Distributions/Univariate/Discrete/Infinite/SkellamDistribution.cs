using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Numerical.Simple;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Infinite;

public class SkellamDistribution : Distribution
{
    public override double Expected => Mean;
    
    public override double Mean => Mu1 - Mu2;
    
    public override double Median => GetMedian();

    public override double Mode => GetMode();

    public override double Variance => Mu1 + Mu2;
    
    public override double Skewness => (Mu1 - Mu2) / Math.Pow(Mu1 + Mu2, 1.5);
    
    public override double Kurtosis => 3 + (1 / (Mu1 + Mu2));

    public override double StandardDeviation => Math.Sqrt(Mu1 + Mu2);
    
    public override double Minimum => double.NegativeInfinity;
    
    public override double Maximum => double.PositiveInfinity;
    
    [EntityParameter(typeof(double), nameof(Probability))]
    public double Mu1 { get; set; }
    
    [EntityParameter(typeof(double), nameof(Probability))]
    public double Mu2 { get; set; }

    private PoissonDistribution _poissonGenerator1;
    private PoissonDistribution _poissonGenerator2;
    
    public override double Distribute()
    {
        var poisson1 = _poissonGenerator1.Distribute();
        var poisson2 = _poissonGenerator2.Distribute();
        
        return poisson1 - poisson2;
    }
    
    public override double Quantile(double p)
    {
        switch (p)
        {
            case < 0:
            case > 1: throw new ArgumentException("p must be between 0 and 1");
            case 0: return Minimum;
            case 1: return Maximum;
        }

        var lower = (int)Math.Floor(Mean - 10 * StandardDeviation);
        var upper = (int)Math.Ceiling(Mean + 10 * StandardDeviation);
        
        lower = Math.Max(lower, -10000);
        upper = Math.Min(upper, 10000);
        
        while (lower < upper)
        {
            var mid = (lower + upper) / 2;
            var cdf = CumulativeDistribution(mid);
            
            if (cdf < p)
                lower = mid + 1;
            else
                upper = mid;
        }
        
        return lower;
    }
    
    public override double ProbabilityDensity(double x)
    {
        if (Math.Abs(x - Math.Round(x)) > 1e-10)
            return 0;
            
        return ProbabilityMass((int)Math.Round(x));
    }
    
    public override double CumulativeDistribution(double x)
    {
        double sum = 0;
        var startK = (int)Math.Floor(Math.Min(x, Mean - 10 * StandardDeviation));
        var endK = (int)Math.Floor(x);
        
        startK = Math.Max(startK, -1000);
        
        for (var k = startK; k <= endK; k++) 
            sum += ProbabilityMass(k);
        
        return sum;
    }
    
    protected override void Validate()
    {
        if (Mu1 <= 0)
            throw new ArgumentException($"Mu1 must be > 0. Current value: {Mu1}");
            
        if (Mu2 <= 0)
            throw new ArgumentException($"Mu2 must be > 0. Current value: {Mu2}");
    }
    
    public double ProbabilityMass(int k)
    {
        if (k < int.MinValue + 1000 || k > int.MaxValue - 1000)
            return 0;
            
        return Math.Exp(-(Mu1 + Mu2)) * Math.Pow(Mu1 / Mu2, k / 2.0) * BesselI(Math.Abs(k), 2 * Math.Sqrt(Mu1 * Mu2));
    }
    
    private double BesselI(int n, double z)
    {
        if (z == 0)
            return (n == 0) ? 1.0 : 0.0;
            
        if (Math.Abs(z) < 20)
        {
            double sum = 0;
            double term;
            var k = 0;
            const int maxTerms = 100;
            
            do
            {
                term = Math.Pow(z / 2, n + 2 * k) /
                       (FactorialFunction.Calculate(k) * FactorialFunction.Calculate(n + k));
                sum += term;
                k++;
            }
            while (k < maxTerms && Math.Abs(term) > 1e-15);
            
            return sum;
        }

        var result = Math.Exp(z) / Math.Sqrt(2 * Math.PI * z);
        double correction = 1;
            
        double m = 4 * n * n;
        correction *= (1 - (m - 1) / (8 * z));
            
        return result * correction;
    }

    private double GetMedian()
    {
        double sum = 0;
        const double target = 0.5;
            
        var startK = (int)Math.Floor(Mean - 5 * StandardDeviation);
        startK = Math.Max(startK, -1000);
            
        for (var k = startK; k <= startK + 2000; k++)
        {
            sum += ProbabilityMass(k);
            if (sum >= target)
                return k;
        }
        return Mean;
    }

    private double GetMode()
    {
        var diff = Mu1 - Mu2;
        var floor = (int)Math.Floor(diff);
        var ceil = (int)Math.Ceiling(diff);
            
        var pFloor = ProbabilityMass(floor);
        var pCeil = ProbabilityMass(ceil);
            
        return pFloor >= pCeil ? floor : ceil;
    }
}