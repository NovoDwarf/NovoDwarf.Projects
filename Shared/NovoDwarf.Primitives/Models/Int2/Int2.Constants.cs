namespace Aegis.Numerics.Primitives;

public readonly partial record struct Int2
{
	public static Int2 Zero => new(0, 0);
	public static Int2 One => new(1, 1);
	
	public static Int2 Up => new(0, 1);
	public static Int2 Down => new(0, -1);
	public static Int2 Left => new(-1, 0);
	public static Int2 Right => new(1, 0);
	
	public static Int2 UnitX => new(1, 0);
	public static Int2 UnitY => new(0, 1);
}