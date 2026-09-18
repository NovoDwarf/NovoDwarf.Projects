namespace NovoDwarf.Primitives.Models.Int3;

public readonly partial record struct Int3
{
	public static Int3 operator -(Int3 value)
	{
		return new Int3(-value.X, -value.Y, -value.Z);
	}

	public static Int3 operator *(Int3 value, float point)
	{
		return new Int3((int)(value.X * point), (int)(value.Y * point), (int)(value.Z * point));
	}

	public static Int3 operator /(Int3 value, float point)
	{
		return new Int3((int)(value.X / point), (int)(value.Y / point), (int)(value.Z / point));
	}

	public static Int3 operator +(Int3 a, Int3 b)
	{
		return new Int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	public static Int3 operator -(Int3 a, Int3 b)
	{
		return new Int3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}

	public static Int3 operator *(Int3 a, int scalar)
	{
		return new Int3(a.X * scalar, a.Y * scalar, a.Z * scalar);
	}
}