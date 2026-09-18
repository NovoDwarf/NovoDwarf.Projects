namespace NovoDwarf.Primitives.Models.Int3;

public readonly partial record struct Int3
{
	public static Int3 Zero => new(0, 0, 0);
	public static Int3 One => new(1, 1, 1);

	public static Int3 UnitX => new(1, 0, 0);
	public static Int3 UnitY => new(0, 1, 0);
	public static Int3 UnitZ => new(0, 0, 1);
}