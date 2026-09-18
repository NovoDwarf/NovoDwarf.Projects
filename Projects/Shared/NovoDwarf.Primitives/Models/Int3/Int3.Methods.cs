namespace NovoDwarf.Primitives.Models.Int3;

public readonly partial record struct Int3
{
	public static int Dot(Int3 a, Int3 b)
	{
		return VMath.Dot(a.X, a.Y, a.Z, b.X, b.Y, b.Z);
	}

	public static Int3 Cross(Int3 a, Int3 b)
	{
		return new Int3(
			VMath.CrossX(a.X, a.Y, a.Z, b.X, b.Y, b.Z),
			VMath.CrossY(a.X, a.Y, a.Z, b.X, b.Y, b.Z),
			VMath.CrossZ(a.X, a.Y, a.Z, b.X, b.Y, b.Z));
	}

	public static int DistanceSquared(Int3 a, Int3 b)
	{
		return (a - b).LengthSquared;
	}

	public static double Distance(Int3 a, Int3 b)
	{
		return (a - b).Length;
	}

	public static Int3 Min(Int3 a, Int3 b)
	{
		return new Int3(VMath.Min(a.X, b.X), VMath.Min(a.Y, b.Y), VMath.Min(a.Z, b.Z));
	}

	public static Int3 Max(Int3 a, Int3 b)
	{
		return new Int3(VMath.Max(a.X, b.X), VMath.Max(a.Y, b.Y), VMath.Max(a.Z, b.Z));
	}
}