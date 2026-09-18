namespace NovoDwarf.Primitives.Models.Float3;

public readonly partial record struct Float3
{
	public static float Dot(Float3 a, Float3 b)
	{
		return VMath.Dot(a.X, a.Y, a.Z, b.X, b.Y, b.Z);
	}

	public static Float3 Cross(Float3 a, Float3 b)
	{
		return new Float3(VMath.CrossX(a.X, a.Y, a.Z, b.X, b.Y, b.Z),
			VMath.CrossY(a.X, a.Y, a.Z, b.X, b.Y, b.Z),
			VMath.CrossZ(a.X, a.Y, a.Z, b.X, b.Y, b.Z));
	}

	public static float DistanceSquared(Float3 a, Float3 b)
	{
		return (a - b).LengthSquared;
	}

	public static float Distance(Float3 a, Float3 b)
	{
		return (a - b).Length;
	}

	public static Float3 Lerp(Float3 a, Float3 b, float t)
	{
		return a + (b - a) * t;
	}

	public static Float3 Min(Float3 a, Float3 b)
	{
		return new Float3(VMath.Min(a.X, b.X), VMath.Min(a.Y, b.Y), VMath.Min(a.Z, b.Z));
	}

	public static Float3 Max(Float3 a, Float3 b)
	{
		return new Float3(VMath.Max(a.X, b.X), VMath.Max(a.Y, b.Y), VMath.Max(a.Z, b.Z));
	}
}