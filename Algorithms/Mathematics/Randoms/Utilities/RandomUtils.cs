using Mathematics.Transforms.Models;

namespace Mathematics.Randoms.Utilities;

/// <summary>
/// A collection of utility methods for random number generation.
/// </summary>
public class RandomUtils
{
	/// <inheritdoc cref="Random.Next()"/>
	public static int Next() => Random.Shared.Next();
	
	/// <inheritdoc cref="Random.Next(int, int)"/>
	public static int Next(int min, int max) => Random.Shared.Next(min, max);

	/// <summary>
	/// Returns a random floating-point number between 0 and 1. Guaranteed to be non-zero.
	/// </summary>
	/// <returns></returns>
	public static double NextDoubleSafe() => 1.0 - Random.Shared.NextDouble();

	/// <inheritdoc cref="Random.NextDouble"/>
	public static double NextDouble() => Random.Shared.NextDouble();

	public static double NextDouble(double min, double max) => min + (max - min) * NextDoubleSafe();

	public static double NextNormal() => BoxMullerTransform.Polar().u;

	public static double NextNormal(double mean, double stdDev) => mean + stdDev * NextNormal();
}