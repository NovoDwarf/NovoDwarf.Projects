namespace Aegis.Numerics.Primitives;

public readonly record struct Float2
{
	public const float NormalizationEpsilon = 1e-4f;

	public Float2(float x, float y)
	{
		X = x;
		Y = y;
	}

	public float X { get; }
	public float Y { get; }

	public static Float2 Zero => new(0f, 0f);
	public static Float2 One => new(1f, 1f);
	public static Float2 UnitX => new(1f, 0f);
	public static Float2 UnitY => new(0f, 1f);

	public float Length => MathF.Sqrt(X * X + Y * Y);
	public float LengthSquared => X * X + Y * Y;

	public Float2 Normalized(float epsilon = NormalizationEpsilon)
	{
		var length = Length;
		return length <= epsilon ? Zero : this / length;
	}

	public bool TryNormalize(out Float2 result, float epsilon = NormalizationEpsilon)
	{
		var length = Length;
		if (length <= epsilon)
		{
			result = Zero;
			return false;
		}

		result = this / length;
		return true;
	}

	public bool NearlyEquals(Float2 other, float epsilon = NormalizationEpsilon)
	{
		return MathF.Abs(X - other.X) <= epsilon &&
		       MathF.Abs(Y - other.Y) <= epsilon;
	}

	public void Deconstruct(out float x, out float y)
	{
		x = X;
		y = Y;
	}

	public override string ToString() => $"({X}, {Y})";

	public static float Dot(Float2 a, Float2 b) => a.X * b.X + a.Y * b.Y;

	public static float Cross(Float2 a, Float2 b) => a.X * b.Y - a.Y * b.X;

	public static float Distance(Float2 a, Float2 b) => (a - b).Length;

	public static float DistanceSquared(Float2 a, Float2 b) => (a - b).LengthSquared;

	public static Float2 Lerp(Float2 a, Float2 b, float t) => a + (b - a) * t;

	public static Float2 Min(Float2 a, Float2 b) => new(
		MathF.Min(a.X, b.X),
		MathF.Min(a.Y, b.Y));

	public static Float2 Max(Float2 a, Float2 b) => new(
		MathF.Max(a.X, b.X),
		MathF.Max(a.Y, b.Y));

	public static Float2 operator +(Float2 a, Float2 b) => new(a.X + b.X, a.Y + b.Y);

	public static Float2 operator -(Float2 a, Float2 b) => new(a.X - b.X, a.Y - b.Y);

	public static Float2 operator -(Float2 value) => new(-value.X, -value.Y);

	public static Float2 operator *(Float2 value, float point) => new(value.X * point, value.Y * point);

	public static Float2 operator *(float point, Float2 value) => value * point;

	public static Float2 operator /(Float2 value, float point)
	{
		if (MathF.Abs(point) <= NormalizationEpsilon)
			throw new DivideByZeroException("Cannot divide Float2 by zero or near-zero Point.");

		return new Float2(value.X / point, value.Y / point);
	}
	
	public bool Equals(Float2 other)
	{
		return X.Equals(other.X) && Y.Equals(other.Y);
	}
	
	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
}