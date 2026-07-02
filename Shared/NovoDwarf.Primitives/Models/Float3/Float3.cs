namespace Aegis.Numerics.Primitives;

public readonly record struct Float3
{
	public const float NormalizationEpsilon = 1e-4f;

	public Float3(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public float X { get; }
	public float Y { get; }
	public float Z { get; }

	public static Float3 Zero => new(0f, 0f, 0f);
	public static Float3 One => new(1f, 1f, 1f);
	public static Float3 UnitX => new(1f, 0f, 0f);
	public static Float3 UnitY => new(0f, 1f, 0f);
	public static Float3 UnitZ => new(0f, 0f, 1f);

	public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);
	public float LengthSquared => X * X + Y * Y + Z * Z;

	public Float3 Normalized(float epsilon = NormalizationEpsilon)
	{
		var length = Length;
		return length <= epsilon ? Zero : this / length;
	}

	public bool TryNormalize(out Float3 result, float epsilon = NormalizationEpsilon)
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

	public bool NearlyEquals(Float3 other, float epsilon = NormalizationEpsilon)
	{
		return MathF.Abs(X - other.X) <= epsilon &&
		       MathF.Abs(Y - other.Y) <= epsilon &&
		       MathF.Abs(Z - other.Z) <= epsilon;
	}

	public void Deconstruct(out float x, out float y, out float z)
	{
		x = X;
		y = Y;
		z = Z;
	}

	public override string ToString() => $"({X}, {Y}, {Z})";

	public static float Dot(Float3 a, Float3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

	public static Float3 Cross(Float3 a, Float3 b) => new(
		a.Y * b.Z - a.Z * b.Y,
		a.Z * b.X - a.X * b.Z,
		a.X * b.Y - a.Y * b.X);

	public static float Distance(Float3 a, Float3 b) => (a - b).Length;

	public static float DistanceSquared(Float3 a, Float3 b) => (a - b).LengthSquared;

	public static Float3 Lerp(Float3 a, Float3 b, float t) => a + (b - a) * t;

	public static Float3 Min(Float3 a, Float3 b) => new(
		MathF.Min(a.X, b.X),
		MathF.Min(a.Y, b.Y),
		MathF.Min(a.Z, b.Z));

	public static Float3 Max(Float3 a, Float3 b) => new(
		MathF.Max(a.X, b.X),
		MathF.Max(a.Y, b.Y),
		MathF.Max(a.Z, b.Z));

	public static Float3 operator +(Float3 a, Float3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

	public static Float3 operator -(Float3 a, Float3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

	public static Float3 operator -(Float3 value) => new(-value.X, -value.Y, -value.Z);

	public static Float3 operator *(Float3 value, float Point) => new(value.X * Point, value.Y * Point, value.Z * Point);

	public static Float3 operator *(float Point, Float3 value) => value * Point;

	public static Float3 operator /(Float3 value, float Point)
	{
		if (MathF.Abs(Point) <= NormalizationEpsilon)
			throw new DivideByZeroException("Cannot divide Float3 by zero or near-zero Point.");

		return new Float3(value.X / Point, value.Y / Point, value.Z / Point);
	}

	public bool Equals(Float3 other)
	{
		return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
	}
}