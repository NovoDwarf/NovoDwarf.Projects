using System;

namespace NovoDwarf.Primitives.Models.Float3;

public readonly partial record struct Float3
{
	public bool Equals(Float3 other)
	{
		return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
	}

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

	public override string ToString()
	{
		return $"({X}, {Y}, {Z})";
	}
}