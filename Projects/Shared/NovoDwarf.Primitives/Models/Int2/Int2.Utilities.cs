using System;

namespace NovoDwarf.Primitives.Models.Int2;

public readonly partial record struct Int2
{
	public static Int2 ToDirection(int fromIndex, int toIndex, int width)
	{
		var fromX = fromIndex % width;
		var fromY = fromIndex / width;
		
		var toX = toIndex % width;
		var toY = toIndex / width;
		
		return new Int2(Math.Sign(toX - fromX), Math.Sign(toY - fromY));
	}

	public static int Dot(Int2 a, Int2 b)
	{
		return a.X * b.X + a.Y * b.Y;
	}

	public static int Cross(Int2 a, Int2 b)
	{
		return a.X * b.Y - a.Y * b.X;
	}

	public static double Distance(Int2 a, Int2 b)
	{
		return (a - b).Length;
	}

	public static int DistanceSquared(Int2 a, Int2 b)
	{
		return (a - b).LengthSquared;
	}

	public static Int2 Min(Int2 a, Int2 b)
	{
		return new Int2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
	}

	public static Int2 Max(Int2 a, Int2 b)
	{
		return new Int2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
	}
}