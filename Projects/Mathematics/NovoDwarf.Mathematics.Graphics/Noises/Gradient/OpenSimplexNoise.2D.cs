using System.Runtime.CompilerServices;

namespace Mathematics.Graphics.Noises.Gradient;

public enum OpenSimplexNoise2DType
{
	Default,
	Unskewed,
	ImproveX
}

public sealed partial class OpenSimplexNoise
{
    private static readonly float[] Gradients2D;
    
    private const double Root2Over2 = 0.7071067811865476;
    private const double Skew2D = 0.366025403784439;
    private const double Unskew2D = -0.21132486540518713;

    private const int NGrads2DExponent = 7;
    private const int NGrads2D = 1 << NGrads2DExponent;
    
    private const double Normalizer2D = 0.01001634121365712;
    private const float Rsquared2D = 0.5f;
    
    public OpenSimplexNoise2DType Type2D { get; set; } = OpenSimplexNoise2DType.Default;
        
    private Func<long, double, double, float>? _float2DFunc;
    
    public float Make(float x, float y)
    {
	    _float2DFunc = Type2D switch
	    {
		    OpenSimplexNoise2DType.Unskewed => Noise2_UnskewedBase,
		    OpenSimplexNoise2DType.ImproveX => Noise2_ImproveX,
		    _ => Noise2
	    };
	    
	    var sx = (x + _options.Offset.X) * _options.Scale;
	    var sy = (y + _options.Offset.Y) * _options.Scale;
	    var value = _float2DFunc(_options.Seed, sx, sy);

	    if (_options.Invert)
		    value = -value;

	    value *= _options.Amplitude;
	    value += _options.Bias;

	    if (_options.Normalize)
		    value = value * 0.5f + 0.5f;

	    if (_options.Power != 1f)
		    value = MathF.Pow(value, _options.Power);

	    if (_options.Clamp01)
		    value = Math.Clamp(value, 0f, 1f);

	    return value;
    }

    private static float Noise2(long seed, double x, double y)
    {
        var s = Skew2D * (x + y);
        double xs = x + s, ys = y + s;

        return Noise2_UnskewedBase(seed, xs, ys);
    }

    private static float Noise2_ImproveX(long seed, double x, double y)
    {
        var xx = x * Root2Over2;
        var yy = y * (Root2Over2 * (1 + 2 * Skew2D));

        return Noise2_UnskewedBase(seed, yy + xx, yy - xx);
    }
	
    private static float Noise2_UnskewedBase(long seed, double xs, double ys)
    {
        int xsb = FastFloor(xs), ysb = FastFloor(ys);
        float xi = (float)(xs - xsb), yi = (float)(ys - ysb);

        long xsbp = xsb * PrimeX, ysbp = ysb * PrimeY;

        var t = (xi + yi) * (float)Unskew2D;
        float dx0 = xi + t, dy0 = yi + t;

        float value = 0;
        var a0 = Rsquared2D - dx0 * dx0 - dy0 * dy0;
        if (a0 > 0)
        {
            value = (a0 * a0) * (a0 * a0) * Grad(seed, xsbp, ysbp, dx0, dy0);
        }

        var a1 = (float)(2 * (1 + 2 * Unskew2D) * (1 / Unskew2D + 2)) * t + ((float)(-2 * (1 + 2 * Unskew2D) * (1 + 2 * Unskew2D)) + a0);
        if (a1 > 0)
        {
            var dx1 = dx0 - (float)(1 + 2 * Unskew2D);
            var dy1 = dy0 - (float)(1 + 2 * Unskew2D);
            value += (a1 * a1) * (a1 * a1) * Grad(seed, xsbp + PrimeX, ysbp + PrimeY, dx1, dy1);
        }

        if (dy0 > dx0)
        {
            var dx2 = dx0 - (float)Unskew2D;
            var dy2 = dy0 - (float)(Unskew2D + 1);
            var a2 = Rsquared2D - dx2 * dx2 - dy2 * dy2;
            if (a2 > 0)
            {
                value += (a2 * a2) * (a2 * a2) * Grad(seed, xsbp, ysbp + PrimeY, dx2, dy2);
            }
        }
        else
        {
            var dx2 = dx0 - (float)(Unskew2D + 1);
            var dy2 = dy0 - (float)Unskew2D;
            var a2 = Rsquared2D - dx2 * dx2 - dy2 * dy2;
            if (a2 > 0)
            {
                value += (a2 * a2) * (a2 * a2) * Grad(seed, xsbp + PrimeX, ysbp, dx2, dy2);
            }
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Grad(long seed, long xsvp, long ysvp, float dx, float dy)
    {
        var hash = seed ^ xsvp ^ ysvp;
        hash *= HashMultiplier;
        hash ^= hash >> (64 - NGrads2DExponent + 1);
        var gi = (int)hash & ((NGrads2D - 1) << 1);
        return Gradients2D[gi | 0] * dx + Gradients2D[gi | 1] * dy;
    }
}