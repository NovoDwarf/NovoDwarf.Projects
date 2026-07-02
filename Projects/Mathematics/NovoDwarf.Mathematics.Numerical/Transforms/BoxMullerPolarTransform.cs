namespace Mathematics.Numerical.Transforms;

public static class BoxMullerPolarTransform
{
	public static (double u, double v) Transform()
	{
		double x, y, s;
		do
		{
			x = 2.0 * Random.Shared.NextDouble() - 1.0;
			y = 2.0 * Random.Shared.NextDouble() - 1.0;
			s = x * x + y * y;
		} while (s is >= 1.0 or 0.0);

		var factor = Math.Sqrt(-2.0 * Math.Log(s) / s);

		return (x * factor, y * factor);
	}
}