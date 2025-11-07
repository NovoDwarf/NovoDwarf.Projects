using Mathematics.Models.Base;
using Mathematics.Models.Distributions.Basic;

namespace Mathematics.Models.Distributions.Derived;

public class Erlang : DistributionBase
{
    public int Shape { get; }
    public double Rate { get; }

    public Erlang(int shape, double rate)
    {
        if (shape < 1)
            throw new ArgumentException("Shape must be greater than 0.", nameof(shape));
        
        if (rate <= 0)
            throw new ArgumentException("Rate must be greater than 0.", nameof(rate));
        
        Shape = shape;
        Rate = rate;
    }

    public override double Calculate()
    {
        var sum = 0.0;
        var exp = new Exponential(Rate);

        for (var i = 0; i < Shape; i++)
            sum += exp.Calculate();

        return sum;
    }

    public override double GetExpectedValue() => Shape / Rate;
    public override double GetVariance() => Shape / (Rate * Rate);
    public override double GetMinValue() => 0;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"Erlang [Shape = {Shape}, Rate = {Rate:F3}]";
}