using System.Numerics;
using Mathematics.Core.Interfaces;

namespace Mathematics.Core.Extensions.Noises;

public static partial class NoiseExtensions
{
	public static float[,] Make(this INoise2D<float> noise, float[] x, float[] y)
	{
		var result = new float[x.Length, y.Length];
			
		for (var i = 0; i < x.Length; i++)
		{
			for (var j = 0; j < y.Length; j++)
			{
				result[i, j] = noise.Make(x[i], y[j]);
			}
		}

		return result;
	}
	
	public static float[,] Make(this INoise2D<float> noise, Vector2 size)
	{
		var width = (int)size.X;
		var height = (int)size.Y;
		
		var result = new float[width, height];

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				result[x, y] = noise.Make(x, y);
			}
		}

		return result;
	}
}