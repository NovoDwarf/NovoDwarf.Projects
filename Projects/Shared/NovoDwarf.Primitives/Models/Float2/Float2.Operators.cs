using System;

namespace NovoDwarf.Primitives.Models.Float2;

public readonly partial record struct Float2
{
	public static Float2 operator +(Float2 a, Float2 b)
	{
		return new Float2(a.X + b.X, a.Y + b.Y);
	}

	public static Float2 operator -(Float2 a, Float2 b)
	{
		return new Float2(a.X - b.X, a.Y - b.Y);
	}

	public static Float2 operator -(Float2 value)
	{
		return new Float2(-value.X, -value.Y);
	}

	public static Float2 operator *(Float2 value, float point)
	{
		return new Float2(value.X * point, value.Y * point);
	}

	public static Float2 operator *(float point, Float2 value)
	{
		return value * point;
	}

	public static Float2 operator /(Float2 value, float point)
	{
		if (MathF.Abs(point) <= NormalizationEpsilon)
			throw new DivideByZeroException("Cannot divide Float2 by zero or near-zero Point.");

		return new Float2(value.X / point, value.Y / point);
	}
}