using Mathematics.Core.Utilities;

namespace Mathematics.Functions.Transforms;

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

public static class BoxMullerTrigonometricTransform
{
	public static (double x, double y) Transform()
	{
		double u1, u2;

		do
		{
			u1 = RandomUtils.NextDouble();
			u2 = RandomUtils.NextDouble();
		} while (u1 == 0.0 || u2 == 0.0);

		var x = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
		var y = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

		return (x, y);
	}
}