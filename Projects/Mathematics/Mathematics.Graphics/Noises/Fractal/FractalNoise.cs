using Mathematics.Core.Interfaces;

namespace Mathematics.Graphics.Noises.Fractal;

/// <summary>
/// 
/// </summary>
public abstract record NoiseOptionsBase
{
	/// <summary>
	/// 
	/// </summary>
	public float Scale { get; init; } = 1.0f;
	
	/// <summary>
	/// 
	/// </summary>
	public float OffsetX { get; init; } = 0f;
	
	/// <summary>
	/// 
	/// </summary>
	public float OffsetY { get; init; } = 0f;
	
	/// <summary>
	/// 
	/// </summary>
	public float OffsetZ { get; init; } = 0f;
	
	/// <summary>
	/// 
	/// </summary>
	public int Seed { get; init; } = 0;
	
	/// <summary>
	/// Gets the gain of the noise
	/// </summary>
	public float Gain { get; init; } = 1.0f;
	
	/// <summary>
	/// Gets the output offset of the noise
	/// </summary>
	public float OutOffset { get; init; } = 0.0f;
	
	/// <summary>
	/// Gets the 
	/// </summary>
	public bool Clamp01 { get; init; } = true;
	
	/// <summary>
	/// Gets
	/// </summary>
	public bool Normalize { get; init; } = true;
	
	/// <summary>
	/// 
	/// </summary>
	public float Power { get; init; } = 1.0f;

	/// <summary>
	/// 
	/// </summary>
	public bool Invert { get; init; } = false;
}


public record FractalNoiseOptions : NoiseOptionsBase
{
	/// <summary>
	/// 
	/// </summary>
	public int Octaves { get; init; } = 6;
	
	/// <summary>
	/// 
	/// </summary>
	public float Persistence { get; init; } = 0.5f;
	
	/// <summary>
	/// 
	/// </summary>
	public float Lacunarity { get; init; } = 2.0f;

	/// <summary>
	/// 
	/// </summary>
	public FractalType Type { get; init; } = FractalType.FBM;
}

public enum FractalType
{
	FBM,
	Billow,
	Ridged,
	Turbulence
}

public class FractalNoise : INoise1D<float>, INoise2D<float>, INoise3D<float>
{
	public FractalNoise(INoise3D<float> noise, FractalNoiseOptions? options = null)
	{
		Options = options ?? new FractalNoiseOptions();
		Noise = noise;
	}

	private INoise3D<float> Noise { get; }
	private FractalNoiseOptions Options { get; }

	public float Make(float x) => Fractal(x, 0, 0);
	public float Make(float x, float y) => Fractal(x, y, 0);
	public float Make(float x, float y, float z) => Fractal(x, y, z);

	private float Fractal(float x, float y, float z)
	{
		x = x * Options.Scale + Options.OffsetX;
		y = y * Options.Scale + Options.OffsetY;
		z = z * Options.Scale + Options.OffsetZ;

		var total = 0f;
		var frequency = 1f;
		var amplitude = 1f;
		var maxValue = 0f;

		for (var i = 0; i < Options.Octaves; i++)
		{
			var n = Noise.Make(
				x * frequency,
				y * frequency,
				z * frequency
			);

			switch (Options.Type)
			{
				case FractalType.Billow:
					n = Math.Abs(n);
					break;

				case FractalType.Ridged:
					n = 1f - Math.Abs(n);
					n *= n;
					break;

				case FractalType.Turbulence:
					n = Math.Abs(n);
					break;
				case FractalType.FBM:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			total += n * amplitude;
			maxValue += amplitude;

			amplitude *= Options.Persistence;
			frequency *= Options.Lacunarity;
		}

		var value = Options.Normalize ? total / maxValue : total;
		
		if (Options.Power != 1f)
			value = MathF.Pow(value, Options.Power);

		if (Options.Invert)
			value = 1f - value;

		value = value * Options.Gain + Options.OutOffset;

		if (!Options.Clamp01) 
			return value;
		
		value = value switch
		{
			< 0 => 0,
			> 1 => 1,
			_ => value
		};

		return value;
	}
}

