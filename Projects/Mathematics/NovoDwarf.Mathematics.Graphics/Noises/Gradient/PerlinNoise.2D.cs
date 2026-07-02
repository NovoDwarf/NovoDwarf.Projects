using Mathematics.Core.Interfaces.Noises;

namespace Mathematics.Graphics.Noises.Gradient;

public partial class PerlinNoise : INoise2D
{
	/// <summary>
	/// Generates a 2D Perlin noise value for a single point.
	/// </summary>
	/// <param name="x">X coordinate of the point</param>
	/// <param name="y">Y coordinate of the point</param>
	/// <returns>Return the noise value at the specified point</returns>
	public float Make(float x, float y)
	{
		float z = 0;
		ApplyInput(ref x, ref y, ref z);

		var X = (int)Math.Floor(x) & (_options.Size - 1);
		var Y = (int)Math.Floor(y) & (_options.Size - 1);

		x -= (float)Math.Floor(x);
		y -= (float)Math.Floor(y);

		var u = Fade(x);
		var v = Fade(y);

		var aa = _permutation[_permutation[X]     + Y];
		var ba = _permutation[_permutation[X + 1] + Y];
		var ab = _permutation[_permutation[X]     + Y + 1];
		var bb = _permutation[_permutation[X + 1] + Y + 1];

		var value =
			Lerp(v,
				Lerp(u,
					Grad(aa, x,     y),
					Grad(ba, x - 1, y)),
				Lerp(u,
					Grad(ab, x,     y - 1),
					Grad(bb, x - 1, y - 1)));

		return ApplyOutput(value);
	}


	private static float Grad(int hash, float x, float y)
	{
		var h = hash & 3;
		return h switch
		{
			0 =>  x + y,
			1 => -x + y,
			2 =>  x - y,
			_ => -x - y
		};
	}
}