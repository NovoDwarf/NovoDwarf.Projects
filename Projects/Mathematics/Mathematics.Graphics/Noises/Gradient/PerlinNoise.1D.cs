using Mathematics.Core.Interfaces;

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
		var floor = (int)Math.Floor(x) & (Size - 1);
		x -= (float)Math.Floor(x);

		var u = Fade(x);

		var a = _permutation[floor];
		var b = _permutation[floor + 1];

		return Lerp(u, Grad1D(a, x), Grad1D(b, x - 1));
	}
	
	/// <summary>
	/// Generates a 1D Perlin noise value for an array of x coordinates.
	/// </summary>
	/// <param name="x">Array of x coordinates</param>
	/// <returns>Array of noise values</returns>
	public float[] Make(float[] x)
	{
		var result = new float[x.Length];
		
		for (var i = 0; i < x.Length; i++)
			result[i] = Make(x[i]);
		
		return result;
	}
	
	private static float Grad1D(int hash, float x)
	{
		var h = hash & 15;
		var g = 1f + (h & 7);
		return ((h & 8) != 0 ? -g : g) * x;
	}
}