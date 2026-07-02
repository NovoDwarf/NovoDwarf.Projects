using Mathematics.Core.Utilities;

namespace Mathematics.Numerical.Transforms;

public static class BoxMullerDecartTransform
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