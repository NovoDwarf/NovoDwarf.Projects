using Mathematics.Core.Interfaces;

namespace Mathematics.Core.Extensions.Noises;

public static partial class NoiseExtensions
{
	public static float[] Make(this INoise1D<float> noise, float[] x)
	{
		var result = new float[x.Length];
			
		for (var i = 0; i < x.Length; i++)
		{
			result[i] = noise.Make(x[i]);
		}

		return result;
	}
		
	public static float[] Make(INoise1D<float> noise, int length)
	{
		var result = new float[length];
		
		for (var x = 0; x < length; x++)
		{
			result[x] = noise.Make(x);
		}
		
		return result;
	}
}