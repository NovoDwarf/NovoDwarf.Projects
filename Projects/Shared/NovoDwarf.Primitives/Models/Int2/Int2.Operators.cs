using System;

namespace NovoDwarf.Primitives.Models.Int2;

public readonly partial record struct Int2
{
	public static Int2 operator +(Int2 a, Int2 b)
	{
		return new Int2(a.X + b.X, a.Y + b.Y);
	}

	public static Int2 operator -(Int2 a, Int2 b)
	{
		return new Int2(a.X - b.X, a.Y - b.Y);
	}

	public static Int2 operator -(Int2 value)
	{
		return new Int2(-value.X, -value.Y);
	}

	public static Int2 operator *(Int2 value, int point)
	{
		return new Int2(value.X * point, value.Y * point);
	}

	public static Int2 operator *(int point, Int2 value)
	{
		return value * point;
	}

	public static Int2 operator /(Int2 value, int point)
	{
		if (point == 0)
			throw new DivideByZeroException("Cannot divide by zero.");

		return new Int2(value.X / point, value.Y / point);
	}
}