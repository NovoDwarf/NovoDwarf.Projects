namespace Mathematics.App.Maui.Systems.Measurements.DTOs;

public sealed class UnitDto
{
	public string Id { get; init; } = string.Empty;

	public string DisplayName { get; init; } = string.Empty;

	public string QuantityId { get; init; } = string.Empty;

	public string SystemId { get; init; } = string.Empty;

	public double Factor { get; init; } = 0;

	public double Offset { get; init; } = 0;

	public bool AllowPrefixes { get; init; } = true;
}