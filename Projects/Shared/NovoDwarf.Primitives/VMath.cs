using System.Numerics;

namespace NovoDwarf.Primitives;

public static class VMath
{
	public static T Dot<T>(T ax, T ay, T bx, T by) where T : INumber<T>
	{
		return ax * bx + ay * by;
	}
	
	public static T Dot<T>(T ax, T ay, T az, T bx, T by, T bz) where T : INumber<T>
	{
		return ax * bx + ay * by + az * bz;
	}
	
	public static T Cross<T>(T ax, T ay, T bx, T by) where T : INumber<T>
	{
		return ax * by - ay * bx;
	}
	
	public static T CrossX<T>(T ax, T ay, T az, T bx, T by, T bz) where T : INumber<T>
	{
		return ay * bz - az * by;
	}

	public static T CrossY<T>(T ax, T ay, T az, T bx, T by, T bz) where T : INumber<T>
	{
		return az * bx - ax * bz;
	}

	public static T CrossZ<T>(T ax, T ay, T az, T bx, T by, T bz) where T : INumber<T>
	{
		return ax * by - ay * bx;
	}
	
	public static T DistanceSquared<T>(T ax, T ay, T bx, T by) where T : INumber<T>
	{
		var dx = ax - bx;
		var dy = ay - by;

		return dx * dx + dy * dy;
	}
	
	public static T DistanceSquared<T>(T ax, T ay, T az, T bx, T by, T bz) where T : INumber<T>
	{
		var dx = ax - bx;
		var dy = ay - by;
		var dz = az - bz;

		return dx * dx + dy * dy + dz * dz;
	}
	
	public static T Min<T>(T a, T b) where T : INumber<T>
	{
		return T.Min(a, b);
	}

	public static T Max<T>(T a, T b) where T : INumber<T>
	{
		return T.Max(a, b);
	}
}