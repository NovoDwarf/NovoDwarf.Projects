using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text.Json.Serialization;
using Mathematics.Graphics.Noises.Gradient;
using Mathematics.Server.Controllers.Noises;

namespace Mathematics.Server.Base.Requests;

public record PerlinNoiseRequest : NoiseRequest
{
	public PerlinNoiseOptions Options { get; init; } = new();
}

public record NoiseRequest
{
	/// <summary>
	/// Width of the generated image in pixels (range: 1 - 4096)
	/// </summary>
	[Range(1, 4096)]
	public int Width { get; init; } = 256;
	
	/// <summary>
	/// Height of the generated image in pixels (range: 1 - 4096)
	/// </summary>
	[Range(1, 4096)]
	public int Height { get; init; } = 256;
	
	/// <summary>
	/// Size of the generated image in pixels (range: 1 - 4096)
	/// </summary>
	[JsonIgnore]
	public Vector2 Size => new(Width, Height);
	
	/// <summary>
	/// Color scheme for the generated image
	/// </summary>
	public ColorSchemeType ColorScheme { get; init; } = ColorSchemeType.Heatmap;
	
	/// <summary>
	/// Image format for the generated image
	/// </summary>
	public ImageFormatType Format { get; init; } = ImageFormatType.Png;
}