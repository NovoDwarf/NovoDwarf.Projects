namespace NovoDwarf.Primitives.Models.Float2;

public readonly partial record struct Float2
{
	public const float NormalizationEpsilon = 1e-4f;

	public static Float2 Zero => new(0f, 0f);
	public static Float2 One => new(1f, 1f);

	public static Float2 Up => new(0, 1);
	public static Float2 Down => new(0, -1);
	public static Float2 Left => new(-1, 0);
	public static Float2 Right => new(1, 0);

	public static Float2 UnitX => new(1f, 0f);
	public static Float2 UnitY => new(0f, 1f);
}