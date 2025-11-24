using Mathematics.Core.Interfaces;

namespace Mathematics.Graphics.Noises.Fractal;

public class FractalNoise
{
	public int Octaves { get; set; } = 6;
	public float Persistence { get; set; } = 0.5f;
	public float Lacunarity { get; set; } = 2.0f;

	public required INoise3D<float> Noiser { get; set; }

	public float Make(float x) => Fractal(x, 0, 0);
	
	public float Make(float x, float y) => Fractal(x, y, 0);

	public float Make(float x, float y, float z) => Fractal(x, y, z);

	private float Fractal(float x, float y, float z)
	{
		float total = 0;
		float frequency = 1;
		float amplitude = 1;
		float maxValue = 0;

		for (var i = 0; i < Octaves; i++)
		{
			total += Noiser.Make(
				x * frequency,
				y * frequency,
				z * frequency
			) * amplitude;

			maxValue += amplitude;

			amplitude *= Persistence;
			frequency *= Lacunarity;
		}

		return total / maxValue;
	}
}
