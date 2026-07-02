using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Utilities;
using Mathematics.Graphics.Noises.Fractal;
using Mathematics.Graphics.Permutations;

namespace Mathematics.Graphics.Noises.Gradient;

public record PerlinNoiseOptions : NoiseOptionsBase
{
    /// <summary>
    /// Size of permutation table. This value must be a power of 2. 
    /// </summary>
    /// <remarks>
    /// This size isn't size of generated noise. It's size of permutation table.
    /// </remarks>
    [Range(1, 4096, ErrorMessage = "Size must be a power of 2.")]
    public int Size { get; init; } = 256;
}

public partial class PerlinNoise
{
    private readonly PermutationTable _permutation;
    private readonly PerlinNoiseOptions _options;
    
    public PerlinNoise(PerlinNoiseOptions? options = null)
    {
	    _options = options ?? new PerlinNoiseOptions();
        _permutation = new PermutationTable(_options.Size, _options.Seed == 0 ? RandomUtils.Next() : _options.Seed);
    }
    
    private void ApplyInput(ref float x, ref float y, ref float z)
    {
        x = x * _options.Scale + _options.Offset.X;
        y = y * _options.Scale + _options.Offset.Y;
        z = z * _options.Scale + _options.Offset.Z;
    }

    private float ApplyOutput(float value)
    {
        if (_options.Invert)
            value = 1f - value;

        if (_options.Power != 1f)
            value = MathF.Pow(MathF.Abs(value), _options.Power);

        value = value * _options.Amplitude + _options.Bias;

        if (!_options.Clamp01) 
            return value;
        
        value = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };

        return value;
    }
    
    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    
    private static float Lerp(float t, float a, float b) => a + t * (b - a);
}