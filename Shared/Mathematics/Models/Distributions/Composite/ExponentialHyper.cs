using Mathematics.Models.Base;
using Mathematics.Models.Distributions.Basic;
using Mathematics.Utilities;

namespace Mathematics.Models.Distributions.Composite;

public class ExponentialHyper : DistributionBase
{
    private readonly double[] _probabilities;
    private readonly Exponential[] _components;

    public double[] Probabilities { get; }
    public double[] Rates { get; }

    public ExponentialHyper(double[]? probabilities, double[]? rates)
    {
        if (probabilities == null || rates == null)
            throw new ArgumentNullException("Probabilities and rates cannot be null");
        
        if (probabilities.Length != rates.Length)
            throw new ArgumentException("Probabilities and rates must have same length");
        
        if (probabilities.Length == 0)
            throw new ArgumentException("At least one component required");
        
        if (probabilities.Any(p => p < 0))
            throw new ArgumentException("Probabilities must be non-negative");
        
        if (rates.Any(r => r <= 0))
            throw new ArgumentException("Rates must be positive");
        
        Probabilities = probabilities;
        Rates = rates;
        
        var sum = Probabilities.Sum();
        
        if (sum <= 0)
            throw new ArgumentException("Sum of probabilities must be positive");
        
        _probabilities = Probabilities.Select(p => p / sum).ToArray();
        _components = Rates.Select(r => new Exponential(r)).ToArray();
    }

    public override double Calculate()
    {
        var r = RandomUtils.NextDouble();
        var cumulative = 0.0;
        var index = _probabilities.Length - 1;

        for (var i = 0; i < _probabilities.Length; i++)
        {
            cumulative += _probabilities[i];
            
            if (r < cumulative || i == _probabilities.Length - 1)
            {
                index = i;
                break;
            }
        }

        return _components[index].Calculate();
    }

    public override double GetExpectedValue() => 
        _probabilities.Zip(_components, (p, c) => p * (1.0 / c.Rates)).Sum();
    
    public override double GetVariance()
    {
        var eX = GetExpectedValue();
        var eX2 = _probabilities.Zip(_components, (p, c) => p * (2.0 / (c.Rates * c.Rates))).Sum();
        return eX2 - eX * eX;
    }
    
    public override double GetMinValue() => 0;
    
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"HyperExponential [Probs = [{string.Join(", ", Probabilities.Select(p => p.ToString("F3")))}], Rates=[{string.Join(", ", Rates.Select(r => r.ToString("F3")))}]]";
}