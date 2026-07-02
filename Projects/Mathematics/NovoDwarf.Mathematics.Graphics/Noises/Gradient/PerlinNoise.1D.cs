using Mathematics.Core.Interfaces.Noises;

namespace Mathematics.Graphics.Noises.Gradient;

public partial class PerlinNoise : INoise1D<float>
{
	/// <summary>
	/// Generates a 1D Perlin noise value at the specified x coordinate.
	/// </summary>
	/// <param name="x">Coordinate of the point</param>
	/// <returns>Return the noise value at the specified point</returns>
	public float Make(float x)
	{
		float y = 0, z = 0;
		ApplyInput(ref x, ref y, ref z);

		var floor = (int)Math.Floor(x) & (_options.Size - 1);
		x -= (float)Math.Floor(x);

		var u = Fade(x);

		var a = _permutation[floor];
		var b = _permutation[floor + 1];

		var value = Lerp(u, Grad(a, x), Grad(b, x - 1));

		return ApplyOutput(value);
	}
	
	private static float Grad(int hash, float x)
	{
		var h = hash & 15;
		var g = 1f + (h & 7);
		return ((h & 8) != 0 ? -g : g) * x;
	}
}