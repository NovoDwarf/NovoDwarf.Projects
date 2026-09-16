namespace NovoDwarf.Primitives.Models;

public readonly record struct ColorF(float R, float G, float B, float A = 1f)
{
	public static ColorF Transparent => new(0f, 0f, 0f, 0f);

	public static ColorF Magenta => new(1f, 0f, 1f);
}