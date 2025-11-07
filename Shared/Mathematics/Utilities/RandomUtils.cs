using Mathematics.Utilities.Formulas;

namespace Mathematics.Utilities;

public class RandomUtils
{
	public static double NextDoubleSafe() => 1.0 - Random.Shared.NextDouble();

	public static double NextDouble() => Random.Shared.NextDouble();
	
	public static double NextDouble(double min, double max) => min + (max - min) * NextDoubleSafe();

	public static double NextDoubleExcluding(Func<double, bool> excludeCondition, int maxAttempts = 1000)
	{
		for (var i = 0; i < maxAttempts; i++)
		{
			var value = NextDouble();
			
			if (!excludeCondition(value))
				return value;
		}
		
		throw new InvalidOperationException($"Failed to generate acceptable random number after [{maxAttempts}] attempts.");
	}
	
	public static double NextNormal() => BoxMullerUtils.BoxMullerPolar().u;

	public static double NextNormal(double mean, double stdDev) => mean + stdDev * NextNormal();
	
	
}