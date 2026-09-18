using System;

namespace NovoDwarf.Primitives.Models.Float2;

public readonly partial record struct Float2
{
	public bool Equals(Float2 other)
	{
		return X.Equals(other.X) && Y.Equals(other.Y);
	}

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
		return MathF.Abs(X - other.X) <= epsilon && MathF.Abs(Y - other.Y) <= epsilon;
	}

	public void Deconstruct(out float x, out float y)
	{
		x = X;
		y = Y;
	}

	public override string ToString()
	{
		return $"Float2({X}, {Y})";
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
}