namespace Mathematics.Core.Utilities;

public class RandomUtils
{
	/// <inheritdoc cref="Random.Next()" />
	public static int Next()
	{
		return Random.Shared.Next();
	}

	/// <inheritdoc cref="Random.Next(int, int)" />
	public static int Next(int min, int max)
	{
		return Random.Shared.Next(min, max);
	}

	/// <summary>
	/// Returns a random floating-point number between 0 and 1. Guaranteed to be non-zero.
	/// </summary>
	/// <returns></returns>
	public static double NextDoubleSafe()
	{
		return 1.0 - Random.Shared.NextDouble();
	}

	/// <inheritdoc cref="Random.NextDouble" />
	public static double NextDouble()
	{
		return Random.Shared.NextDouble();
	}

	public static double NextDouble(double min, double max)
	{
		return min + (max - min) * NextDoubleSafe();
	}
}