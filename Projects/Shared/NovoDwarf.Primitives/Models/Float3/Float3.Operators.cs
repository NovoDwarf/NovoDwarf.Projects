using System;

namespace NovoDwarf.Primitives.Models.Float3;

public readonly partial record struct Float3
{
	public static Float3 operator +(Float3 a, Float3 b)
	{
		return new Float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	public static Float3 operator -(Float3 a, Float3 b)
	{
		return new Float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}

	public static Float3 operator -(Float3 value)
	{
		return new Float3(-value.X, -value.Y, -value.Z);
	}

	public static Float3 operator *(Float3 value, float point)
	{
		return new Float3(value.X * point, value.Y * point, value.Z * point);
	}

	public static Float3 operator *(float point, Float3 value)
	{
		return value * point;
	}

	public static Float3 operator /(Float3 value, float point)
	{
		if (MathF.Abs(point) <= NormalizationEpsilon)
			throw new DivideByZeroException("Cannot divide Float3 by zero or near-zero Point.");

		return new Float3(value.X / point, value.Y / point, value.Z / point);
	}
}