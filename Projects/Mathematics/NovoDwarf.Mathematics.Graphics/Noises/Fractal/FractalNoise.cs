using System.Numerics;
using Mathematics.Core.Interfaces.Noises;

namespace Mathematics.Graphics.Noises.Fractal;

public record OpenSimplexOptions : NoiseOptionsBase;

public abstract record NoiseOptionsBase
{
	public float Scale { get; init; } = 1.0f;
	
	public Vector4 Offset { get; init; } = new Vector4(0, 0, 0, 0);

	public long Seed { get; init; } = 0;

	public float Amplitude { get; init; } = 1.0f;

	public float Bias { get; init; } = 0.0f;
	
	public bool Clamp01 { get; init; } = true;

	public bool Normalize { get; init; } = true;

	public float Power { get; init; } = 1.0f;

	public bool Invert { get; init; } = false;
}


public record FractalNoiseOptions : NoiseOptionsBase
{
	
	public int Octaves { get; init; } = 6;

	public float Persistence { get; init; } = 0.5f;

	public float Lacunarity { get; init; } = 2.0f;

	public FractalType Type { get; init; } = FractalType.FBM;
}

public enum FractalType
{
	FBM,
	Billow,
	Ridged,
	Turbulence
}

public class FractalNoise : INoise1D<float>, INoise2D, INoise3D
{
	public FractalNoise(INoise3D noise, FractalNoiseOptions? options = null)
	{
		Options = options ?? new FractalNoiseOptions();
		Noise = noise;
	}

	private INoise3D Noise { get; }
	private FractalNoiseOptions Options { get; }

	public float Make(float x) => Fractal(x, 0, 0);
	public float Make(float x, float y) => Fractal(x, y, 0);
	public float Make(float x, float y, float z) => Fractal(x, y, z);

	private float Fractal(float x, float y, float z)
	{
		x = x * Options.Scale + Options.Offset.X;
		y = y * Options.Scale + Options.Offset.Y;
		z = z * Options.Scale + Options.Offset.Z;

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

		value = value * Options.Amplitude + Options.Bias;

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

