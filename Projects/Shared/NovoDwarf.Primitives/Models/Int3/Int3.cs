using System;

namespace NovoDwarf.Primitives.Models.Int3;

public readonly partial record struct Int3
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
}