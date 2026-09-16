using System;

namespace NovoDwarf.Primitives.Models.Int3;

public readonly record struct Int3
{
	public Int3(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public int X { get; }
	public int Y { get; }
	public int Z { get; }

	public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
	public int LengthSquared => X * X + Y * Y + Z * Z;

	public static Int3 operator +(Int3 a, Int3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static Int3 operator -(Int3 a, Int3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static Int3 operator *(Int3 a, int scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

	public override string ToString() => $"({X}, {Y}, {Z})";
}
