using System;

namespace NovoDwarf.Primitives.Models.Float3;

public readonly partial record struct Float3
{
	public Float3(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public float X { get; }
	public float Y { get; }
	public float Z { get; }

	public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);
	public float LengthSquared => X * X + Y * Y + Z * Z;
}