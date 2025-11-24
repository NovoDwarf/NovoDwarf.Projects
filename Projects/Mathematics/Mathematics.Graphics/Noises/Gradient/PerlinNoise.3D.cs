using Mathematics.Core.Interfaces;

namespace Mathematics.Graphics.Noises.Gradient;

public partial class PerlinNoise : INoise3D<float>
{
	public float Make(float x, float y, float z)
	{
		ApplyInput(ref x, ref y, ref z);

		var X = (int)Math.Floor(x) & (Size - 1);
		var Y = (int)Math.Floor(y) & (Size - 1);
		var Z = (int)Math.Floor(z) & (Size - 1);

		x -= (float)Math.Floor(x);
		y -= (float)Math.Floor(y);
		z -= (float)Math.Floor(z);

		var u = Fade(x);
		var v = Fade(y);
		var w = Fade(z);

		var a = _permutation.Hash(X) + Y;
		var aa = _permutation.Hash(a) + Z;
		var ab = _permutation.Hash(a + 1) + Z;
		var b = _permutation.Hash(X + 1) + Y;
		var ba = _permutation.Hash(b) + Z;
		var bb = _permutation.Hash(b + 1) + Z;

		var value =
			Lerp(w,
				Lerp(v,
					Lerp(u, Grad(_permutation[aa], x,     y,     z),
                        Grad(_permutation[ba], x - 1, y,     z)),
					Lerp(u, Grad(_permutation[ab], x,     y - 1, z),
                        Grad(_permutation[bb], x - 1, y - 1, z))),
				Lerp(v,
					Lerp(u, Grad(_permutation[aa + 1], x,     y,     z - 1),
                        Grad(_permutation[ba + 1], x - 1, y,     z - 1)),
					Lerp(u, Grad(_permutation[ab + 1], x,     y - 1, z - 1),
                        Grad(_permutation[bb + 1], x - 1, y - 1, z - 1))));

		return ApplyOutput(value);
	}
	
	private static float Grad(int hash, float x, float y, float z)
	{
		var h = hash & 15;
		var u = h < 8 ? x : y;
		var v = h < 4 ? y : h == 12 || h == 14 ? x : z;
		
		return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
	}
}