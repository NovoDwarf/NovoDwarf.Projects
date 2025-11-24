using System.ComponentModel.DataAnnotations;

namespace Mathematics.Core.Parameters;

public record NoiseParameters
{
	[Range(0, 1)]
	public float Scale { get; init; } = 0.01f;
}