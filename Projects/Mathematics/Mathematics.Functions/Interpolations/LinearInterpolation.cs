using System.Drawing;
using System.Runtime.CompilerServices;

namespace Mathematics.Functions.Interpolations;

public static class LinearInterpolation
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Interpolate(float a, float b, float t) => (1 - t) * a + t * b;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Interpolate(double a, double b, double t) => (1 - t) * a + t * b;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Interpolate(decimal a, decimal b, decimal t) => (1 - t) * a + t * b;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color Interpolate(Color a, Color b, float t)
	{
		return Color.FromArgb(
			(int)(a.R + (b.R - a.R) * t),
			(int)(a.G + (b.G - a.G) * t),
			(int)(a.B + (b.B - a.B) * t)
		);
	}
}