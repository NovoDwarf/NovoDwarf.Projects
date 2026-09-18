using System;

namespace NovoDwarf.Primitives.Models.Float2;

public readonly partial record struct Float2
{
	public Float2(float x, float y)
	{
		X = x;
		Y = y;
	}

	public float X { get; }
	public float Y { get; }

	public float Length => MathF.Sqrt(X * X + Y * Y);
	public float LengthSquared => X * X + Y * Y;
}