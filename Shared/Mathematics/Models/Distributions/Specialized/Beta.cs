using Mathematics.Models.Base;
using Mathematics.Models.Distributions.Composite;

namespace Mathematics.Models.Distributions.Specialized;

public class Beta : DistributionBase
{
    private readonly Gamma _gammaAlpha;
    private readonly Gamma _gammaBeta;
    
    public double AlphaParam { get; }
    public double BetaParam { get; }
    public double ScaleParam { get; }
    
    public Beta(double alphaParam, double betaParam, double scaleParam)
    {
        AlphaParam = alphaParam;
        BetaParam = betaParam;
        ScaleParam = scaleParam;
        
        _gammaAlpha = new Gamma(AlphaParam, ScaleParam);
        _gammaBeta = new Gamma(BetaParam, ScaleParam);
    }
    
    public override double Calculate()
    {
        var y1 = _gammaAlpha.Calculate();
        var y2 = _gammaBeta.Calculate();

        return y1 / (y1 + y2);
    }

    public override double GetExpectedValue() => AlphaParam / (AlphaParam + BetaParam);
    public override double GetVariance() => AlphaParam * BetaParam / ((AlphaParam + BetaParam) * (AlphaParam + BetaParam) * (AlphaParam + BetaParam + 1));
    public override double GetMinValue() => 0;
    public override double GetMaxValue() => 1;

    public override string ToString() => $"Beta [α = {AlphaParam:F3}, β = {BetaParam:F3}, Scale = {ScaleParam:F3}]";
}