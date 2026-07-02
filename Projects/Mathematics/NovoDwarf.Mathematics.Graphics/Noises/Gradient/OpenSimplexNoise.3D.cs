using System.Runtime.CompilerServices;

namespace Mathematics.Graphics.Noises.Gradient;

public enum OpenSimplexNoise3DType
{
    Unrotated = 0,
    ImproveXY,
    ImproveXZ,
    Fallback
}

public partial class OpenSimplexNoise
{
	private static readonly float[] Gradients3D;
	
    private const double Root3Over3 = 0.577350269189626;
    private const double FallbackRotate3D = 2.0 / 3.0;
    private const double Rotate3DOrthogonalizer = Unskew2D;
    
    private const int NGrads3DExponent = 8;
    private const int NGrads3D = 1 << NGrads3DExponent;
    
    private const double Normalizer3D = 0.07969837668935331;
    private const float Rsquared3D = 0.6f;
    
    public OpenSimplexNoise3DType Type3D { get; set; } = OpenSimplexNoise3DType.Unrotated;
        
    private Func<long, double, double, double, float>? _float3DFunc;
    
    public float Make(float x, float y, float z)
    {
	    _float3DFunc = Type3D switch
	    {
		    OpenSimplexNoise3DType.Unrotated => Noise3_UnrotatedBase,
		    OpenSimplexNoise3DType.ImproveXY => Noise3_ImproveXY,
		    OpenSimplexNoise3DType.ImproveXZ => Noise3_ImproveXZ,
		    _ => Noise3_Fallback
	    };
	    
	    var sx = (x + _options.Offset.X) * _options.Scale;
	    var sy = (y + _options.Offset.Y) * _options.Scale;
	    var sz = (z + _options.Offset.Z) * _options.Scale;
	    
	    var value = _float3DFunc(_options.Seed, sx, sy, sz);

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

    private static float Noise3_ImproveXY(long seed, double x, double y, double z)
    {
        var xy = x + y;
        var s2 = xy * Rotate3DOrthogonalizer;
        var zz = z * Root3Over3;
        var xr = x + s2 + zz;
        var yr = y + s2 + zz;
        var zr = xy * -Root3Over3 + zz;

        return Noise3_UnrotatedBase(seed, xr, yr, zr);
    }

    private static float Noise3_ImproveXZ(long seed, double x, double y, double z)
    {
        var xz = x + z;
        var s2 = xz * Rotate3DOrthogonalizer;
        var yy = y * Root3Over3;
        var xr = x + s2 + yy;
        var zr = z + s2 + yy;
        var yr = xz * -Root3Over3 + yy;

        return Noise3_UnrotatedBase(seed, xr, yr, zr);
    }
    
    public static float Noise3_Fallback(long seed, double x, double y, double z)
    {
        var r = FallbackRotate3D * (x + y + z);
        double xr = r - x, yr = r - y, zr = r - z;
        
        return Noise3_UnrotatedBase(seed, xr, yr, zr);
    }
    
    private static float Noise3_UnrotatedBase(long seed, double xr, double yr, double zr)
    {
        int xrb = FastRound(xr), yrb = FastRound(yr), zrb = FastRound(zr);
        float xri = (float)(xr - xrb), yri = (float)(yr - yrb), zri = (float)(zr - zrb);

        int xNSign = (int)(-1.0f - xri) | 1, yNSign = (int)(-1.0f - yri) | 1, zNSign = (int)(-1.0f - zri) | 1;

        float ax0 = xNSign * -xri, ay0 = yNSign * -yri, az0 = zNSign * -zri;

        long xrbp = xrb * PrimeX, yrbp = yrb * PrimeY, zrbp = zrb * PrimeZ;

        float value = 0;
        var a = (Rsquared3D - xri * xri) - (yri * yri + zri * zri);
        
        for (var l = 0; ; l++)
        {
            if (a > 0)
            {
                value += (a * a) * (a * a) * Grad(seed, xrbp, yrbp, zrbp, xri, yri, zri);
            }

            if (ax0 >= ay0 && ax0 >= az0)
            {
                var b = a + ax0 + ax0;
                if (b > 1)
                {
                    b -= 1;
                    value += (b * b) * (b * b) * Grad(seed, xrbp - xNSign * PrimeX, yrbp, zrbp, xri + xNSign, yri, zri);
                }
            }
            else if (ay0 > ax0 && ay0 >= az0)
            {
                var b = a + ay0 + ay0;
                if (b > 1)
                {
                    b -= 1;
                    value += (b * b) * (b * b) * Grad(seed, xrbp, yrbp - yNSign * PrimeY, zrbp, xri, yri + yNSign, zri);
                }
            }
            else
            {
                var b = a + az0 + az0;
                if (b > 1)
                {
                    b -= 1;
                    value += (b * b) * (b * b) * Grad(seed, xrbp, yrbp, zrbp - zNSign * PrimeZ, xri, yri, zri + zNSign);
                }
            }

            if (l == 1) break;

            ax0 = 0.5f - ax0;
            ay0 = 0.5f - ay0;
            az0 = 0.5f - az0;

            xri = xNSign * ax0;
            yri = yNSign * ay0;
            zri = zNSign * az0;

            a += (0.75f - ax0) - (ay0 + az0);

            xrbp += (xNSign >> 1) & PrimeX;
            yrbp += (yNSign >> 1) & PrimeY;
            zrbp += (zNSign >> 1) & PrimeZ;

            xNSign = -xNSign;
            yNSign = -yNSign;
            zNSign = -zNSign;

            seed ^= SeedFlip3D;
        }

        return value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Grad(long seed, long xrvp, long yrvp, long zrvp, float dx, float dy, float dz)
    {
        var hash = (seed ^ xrvp) ^ (yrvp ^ zrvp);
        hash *= HashMultiplier;
        hash ^= hash >> (64 - NGrads3DExponent + 2);
        var gi = (int)hash & ((NGrads3D - 1) << 2);
        return Gradients3D[gi | 0] * dx + Gradients3D[gi | 1] * dy + Gradients3D[gi | 2] * dz;
    }
}