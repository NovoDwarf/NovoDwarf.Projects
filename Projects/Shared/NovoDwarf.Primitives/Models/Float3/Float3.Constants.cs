namespace NovoDwarf.Primitives.Models.Float3;

public readonly partial record struct Float3
{
	public const float NormalizationEpsilon = 1e-4f;

	public static Float3 Zero => new(0f, 0f, 0f);
	public static Float3 One => new(1f, 1f, 1f);

	public static Float3 UnitX => new(1f, 0f, 0f);
	public static Float3 UnitY => new(0f, 1f, 0f);
	public static Float3 UnitZ => new(0f, 0f, 1f);
}