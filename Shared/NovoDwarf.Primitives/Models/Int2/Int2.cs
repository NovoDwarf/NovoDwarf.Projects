namespace Aegis.Numerics.Primitives;

public readonly partial record struct Int2
{
	public Int2(int x, int y)
	{
		X = x;
		Y = y;
	}

	public int X { get; }
	public int Y { get; }
	
	public double Length => Math.Sqrt(X * X + Y * Y);
	public int LengthSquared => X * X + Y * Y;


	

}