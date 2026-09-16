using System;

namespace NovoDwarf.Primitives.Models.Int2;

public readonly partial record struct Int2
{
	public static Int2 operator +(Int2 a, Int2 b) => new(a.X + b.X, a.Y + b.Y);

	public static Int2 operator -(Int2 a, Int2 b) => new(a.X - b.X, a.Y - b.Y);

	public static Int2 operator -(Int2 value) => new(-value.X, -value.Y);

	public static Int2 operator *(Int2 value, int point) => new(value.X * point, value.Y * point);

	public static Int2 operator *(int point, Int2 value) => value * point;

	public static Int2 operator /(Int2 value, int point)
	{
		return point != 0 
			? new Int2(value.X / point, value.Y / point) 
			: throw new DivideByZeroException("Cannot divide by zero.");
	}
}