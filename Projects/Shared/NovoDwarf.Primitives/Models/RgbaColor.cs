using System;

namespace NovoDwarf.Primitives.Models;

public sealed record RgbaColor(float R, float G, float B, float A)
{
	public static RgbaColor White { get; } = new(1f, 1f, 1f, 1f);

	public RgbaColor Clamp()
	{
		return new RgbaColor(
			Math.Clamp(R, 0f, 1f),
			Math.Clamp(G, 0f, 1f),
			Math.Clamp(B, 0f, 1f),
			Math.Clamp(A, 0f, 1f));
	}
}
