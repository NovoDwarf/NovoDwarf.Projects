using Mathematics.Core.Base;
using Mathematics.Core.Utilities;
using Mathematics.Functions;
using Mathematics.Functions.Transforms;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class GammaDistribution : Distribution
{
    private readonly double _shape;
    private readonly double _scale;
    
    public GammaDistribution(double shape = 2, double scale = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(shape, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0);

        _shape = shape;
        _scale = scale;
    }

    public override double Expected => _shape * _scale;
    
    public override double Mean => _shape * _scale;
    
    public override double Median
    {
        get
        {
            // Медиана гамма-распределения не имеет простой аналитической формы
            // Используем приближение через обратную неполную гамма-функцию
            if (_shape == 1)
                return _scale * Math.Log(2); // Для экспоненциального распределения
            
            // Приближение для медианы
            return _scale * (_shape - 1.0 / 3.0);
        }
    }
    
    public override double Mode
    {
        get
        {
            if (_shape >= 1)
                return (_shape - 1) * _scale;
            else
                return 0;
        }
    }
    
    public override double Variance => _shape * _scale * _scale;
    
    public override double Skewness => 2.0 / Math.Sqrt(_shape);
    
    public override double Kurtosis => 6.0 / _shape;
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;

    public override double Distribute()
    {
        return _shape >= 1.0
            ? CalculateGamma(_shape) * _scale
            : CalculateSmallShapeGamma(_shape) * _scale;
    }

    public override double Quantile(double p)
    {
        if (p < 0 || p > 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return 0;
            case 1: return double.PositiveInfinity;
        }
        
        double low = 0;
        double high = 1;
        
        while (CumulativeDistribution(high) < p)
        {
            high *= 2;
        }

        const double tolerance = 1e-10;
        const int maxIterations = 100;

        for (var i = 0; i < maxIterations; i++)
        {
            var mid = (low + high) / 2;
            var fmid = CumulativeDistribution(mid);

            if (Math.Abs(fmid - p) < tolerance)
                return mid;

            if (fmid < p)
                low = mid;
            else
                high = mid;
        }

        return (low + high) / 2;
    }
    
    public override double ProbabilityDensity(double x)
    {
        if (x < 0)
            return 0;

        if (x == 0)
        {
            return _shape switch
            {
                < 1 => double.PositiveInfinity,
                1 => 1.0 / _scale,
                _ => 0
            };
        }

        // f(x) = (1/(Γ(k)θ^k)) * x^(k-1) * e^(-x/θ)
        var numerator = Math.Pow(x, _shape - 1) * Math.Exp(-x / _scale);
        var denominator = GammaFunction.Calculate(_shape) * Math.Pow(_scale, _shape);
        
        return numerator / denominator;
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        // F(x) = γ(k, x/θ) / Γ(k)
        return LowerIncompleteGamma(_shape, x / _scale) / GammaFunction.Calculate(_shape);
    }
    
    private static double LowerIncompleteGamma(double s, double x)
    {
        // Нижняя неполная гамма-функция через численное интегрирование
        if (x == 0) 
            return 0;
        if (double.IsPositiveInfinity(x)) 
            return GammaFunction.Calculate(s);

        const int steps = 1000;
        double sum = 0;
        var step = x / steps;

        for (var i = 0; i < steps; i++)
        {
            var t = (i + 0.5) * step;
            sum += Math.Pow(t, s - 1) * Math.Exp(-t);
        }

        return sum * step;
    }

    private double CalculateGamma(double alpha)
    {
        var d = alpha - 1.0 / 3.0;
        var c = 1.0 / Math.Sqrt(9.0 * d);

        while (true)
        {
            double x;
            do
            {
                x = BoxMullerPolarTransform.Transform().u;
            } while (x <= -1.0 / c);

            var v = 1.0 + c * x;
            v = v * v * v;

            var u = RandomUtils.NextDouble();

            if (u < 1.0 - 0.0331 * Math.Pow(x, 4) ||
                Math.Log(u) < 0.5 * x * x + d * (1.0 - v + Math.Log(v)))
                return d * v;
        }
    }

    private double CalculateSmallShapeGamma(double alpha)
    {
        while (true)
        {
            var u = RandomUtils.NextDouble();
            var v = RandomUtils.NextDouble();

            var x = Math.Pow(u, 1.0 / alpha);
            var y = Math.Pow(v, 1.0 / (1.0 - alpha));

            if (!(x + y <= 1.0))
                continue;

            var z = x / (x + y);
            var w = -Math.Log(Random.Shared.NextDouble());

            return z * w;
        }
    }

    public double Shape => _shape;
    public double Scale => _scale;

    public override string ToString() => $"Gamma Distribution [Shape = {_shape}, Scale = {_scale}]";

    public override bool Equals(object? obj) => obj is GammaDistribution other && _shape == other._shape && _scale == other._scale;

    public override int GetHashCode() => HashCode.Combine(_shape, _scale);
}