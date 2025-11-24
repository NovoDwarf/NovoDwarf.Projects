using System.Numerics;
using Mathematics.Core.Interfaces;
using Mathematics.Core.Parameters;

namespace Mathematics.Core.Extensions.Noises;



public static partial class NoiseExtensions
{
	public static float[,] Make(this INoise2D<float> noise, float[] x, float[] y, NoiseParameters? param = null)
	{
		param ??= new NoiseParameters();
		
		var result = new float[x.Length, y.Length];
			
		for (var i = 0; i < x.Length; i++)
		{
			for (var j = 0; j < y.Length; j++)
			{
				result[i, j] = noise.Make(x[i] * param.Scale, y[j] * param.Scale);
			}
		}

		return result;
	}
	
	public static float[,] Make(this INoise2D<float> noise, Vector2 size, NoiseParameters? param = null)
	{
		param ??= new NoiseParameters();
		
		var width = (int)size.X;
		var height = (int)size.Y;
		
		var result = new float[width, height];

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				result[x, y] = noise.Make(x * param.Scale, y * param.Scale);
			}
		}

		return result;
	}
}