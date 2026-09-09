namespace SultanDynasty.Simulation.Generators;

internal static class GenerationRandom
{
	extension(Random random)
	{
		public float NextFloat(float min, float max)
		{
			return min + (float)random.NextDouble() * (max - min);
		}

		public T Pick<T>(IReadOnlyList<T> values)
		{
			return values[random.Next(values.Count)];
		}

		public T PickEnum<T>() where T : struct, Enum
		{
			var values = Enum.GetValues<T>();
			return values[random.Next(values.Length)];
		}
	}
}
