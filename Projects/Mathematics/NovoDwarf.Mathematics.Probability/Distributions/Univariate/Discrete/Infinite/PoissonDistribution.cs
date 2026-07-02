using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;
using Mathematics.Numerical.Simple;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Infinite;

[Categories("Distributions", "Univariate", "Discrete", "Infinite")]
public partial class PoissonDistribution : Distribution
{
    public override double Expected => Lambda;
    
    public override double Mean => Lambda;
    
    public override double Median => Math.Floor(Lambda + 1.0/3 - 0.02/Lambda);
    
    public override double Mode => Math.Floor(Lambda);
    
    public override double Variance => Lambda;
    
    public override double Skewness => 1.0 / Math.Sqrt(Lambda);
    
    public override double Kurtosis => 3.0 + 1.0 / Lambda;
    
    public override double StandardDeviation => Math.Sqrt(Lambda);
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;
    
    [EntityParameter(typeof(double), nameof(Lambda))]
    public double Lambda { get; set; }
    
    public override double Distribute()
    {
        if (Lambda < 30)
        {
            var l = Math.Exp(-Lambda);
            var k = 0;
            var p = 1.0;
            
            do
            {
                k++;
                p *= RandomUtils.NextDouble();
            }
            while (p > l);
            
            return k - 1;
        }

        var u1 = 1.0 - RandomUtils.NextDouble();
        var u2 = 1.0 - RandomUtils.NextDouble();
        
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        var randNormal = Math.Sqrt(Lambda) * randStdNormal + Lambda;
        
        return Math.Max(0, Math.Round(randNormal));
    }
    
    public override double Quantile(double p)
    {
        if (p is < 0 or > 1)
            throw new ArgumentException("p must be between 0 and 1");
            
        if (p == 0) return 0;
        if (p == 1) return double.PositiveInfinity;
        
        double sum = 0;
        var k = 0;
        var l = Math.Exp(-Lambda);
        
        while (k < 1000)
        {
            sum += Math.Pow(Lambda, k) * l / FactorialFunction.Calculate(k);
            if (sum >= p)
                return k;
            k++;
        }
        
        return k;
    }
    
    public override double ProbabilityDensity(double x)
    {
        if (x < 0 || Math.Abs(x - Math.Round(x)) > 1e-10)
            return 0;
            
        var k = (int)Math.Round(x);
        return Math.Pow(Lambda, k) * Math.Exp(-Lambda) / FactorialFunction.Calculate(k);
    }
    
    public override double CumulativeDistribution(double x)
    {
        if (x < 0) return 0;
        
        var k = (int)Math.Floor(x);
        double sum = 0;
        var l = Math.Exp(-Lambda);
        
        for (var i = 0; i <= k; i++)
        {
            sum += Math.Pow(Lambda, i) * l / FactorialFunction.Calculate(i);
        }
        
        return sum;
    }
    
    protected override void Validate()
    {
        if (Lambda <= 0)
            throw new ArgumentException("Lambda must be > 0");
    }
}