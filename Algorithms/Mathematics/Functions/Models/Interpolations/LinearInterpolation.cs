namespace Mathematics.Functions.Models.Interpolations;

public class LinearInterpolation
{
	public float Interpolate(float a, float b, float t)
	{
		return a + t * (b - a);
	}
}