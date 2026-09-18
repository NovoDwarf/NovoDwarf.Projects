namespace NovoDwarf.Primitives.Models.Float2;

public readonly partial record struct Float2
{
	public static float Dot(Float2 a, Float2 b)
	{
		return VMath.Dot(a.X, a.Y, b.X, b.Y);
	}

	public static float Cross(Float2 a, Float2 b)
	{
		return VMath.Cross(a.X, a.Y, b.X, b.Y);
	}

	public static float DistanceSquared(Float2 a, Float2 b)
	{
		return (a - b).LengthSquared;
	}

	public static float Distance(Float2 a, Float2 b)
	{
		return (a - b).Length;
	}

	public static Float2 Lerp(Float2 a, Float2 b, float t)
	{
		return a + (b - a) * t;
	}

	public static Float2 Min(Float2 a, Float2 b)
	{
		return new Float2(VMath.Min(a.X, b.X), VMath.Min(a.Y, b.Y));
	}

	public static Float2 Max(Float2 a, Float2 b)
	{
		return new Float2(VMath.Max(a.X, b.X), VMath.Max(a.Y, b.Y));
	}
}