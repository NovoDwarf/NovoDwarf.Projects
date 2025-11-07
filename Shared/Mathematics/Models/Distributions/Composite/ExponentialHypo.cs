using Mathematics.Models.Base;
using Mathematics.Models.Distributions.Basic;

namespace Mathematics.Models.Distributions.Composite;

public class ExponentialHypo : DistributionBase
{
    private readonly Exponential[] _stages;

    public double[] Rates { get; }

    public ExponentialHypo(double[] rates)
    {
        if (rates == null || rates.Length == 0)
            throw new ArgumentException("At least one rate is required", nameof(rates));
    
        if (rates.Any(r => r <= 0))
            throw new ArgumentException("All rates must be positive", nameof(rates));
        
        Rates = rates;
        
        _stages = rates.Select(rate => new Exponential(rate)).ToArray();
    }
    
    public override double Calculate()
    {
        return _stages.Sum(stage => stage.Calculate());
    }

    public override double GetExpectedValue() => _stages.Sum(s => s.GetExpectedValue());
    public override double GetVariance() => _stages.Sum(s => s.GetVariance());
    public override double GetMinValue() => 0;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"HypoExponential(rates=[{string.Join(", ", Rates.Select(r => r.ToString("F3")))}])";
}