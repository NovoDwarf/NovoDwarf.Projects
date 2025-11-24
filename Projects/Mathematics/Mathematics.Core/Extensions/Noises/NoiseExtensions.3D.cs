using System.Numerics;
using Mathematics.Core.Interfaces;

namespace Mathematics.Core.Extensions.Noises;

public static partial class NoiseExtensions
{
	public static float[,,] Make(this INoise3D<float> noise, float[] x, float[] y, float[] z)
	{
		var result = new float[x.Length, y.Length, z.Length];
			
		for (var i = 0; i < x.Length; i++)
		{
			for (var j = 0; j < y.Length; j++)
			{
				for (var k = 0; k < z.Length; k++)
				{
					result[i, j, k] = noise.Make(x[i], y[j], z[k]);
				}
			}
		}

		return result;
	}
		
	public static float[,,] Make(INoise3D<float> noise, Vector3 size)
	{
		var width = (int)size.X;
		var height = (int)size.Y;
		var depth = (int)size.Z;
		
		var result = new float[width, height, depth];

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				for (var z = 0; z < depth; z++)
				{
					result[x, y, z] = noise.Make(x, y, z);
				}
			}
		}

		return result;
	}
}