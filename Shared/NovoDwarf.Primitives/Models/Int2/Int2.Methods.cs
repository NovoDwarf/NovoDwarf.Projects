namespace Aegis.Numerics.Primitives;

public readonly partial record struct Int2
{
	public void Deconstruct(out int x, out int y)
	{
		x = X;
		y = Y;
	}
	
	public bool NearlyEquals(Int2 other, int epsilon = 0)
	{
		return Math.Abs(X - other.X) <= epsilon && Math.Abs(Y - other.Y) <= epsilon;
	}

	public bool Equals(Int2 other)
	{
		return X == other.X && Y == other.Y;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
	
	public override string ToString()
	{
		return $"({X}, {Y})";
	}
}