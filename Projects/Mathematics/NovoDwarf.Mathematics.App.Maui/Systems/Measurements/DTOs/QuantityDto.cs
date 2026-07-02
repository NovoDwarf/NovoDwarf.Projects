namespace NovoDwarf.Mathematics.App.Systems.Measurements.DTOs;

public sealed class QuantityDto
{
	public string Id { get; init; } = string.Empty;

	public string DisplayName { get; init; } = string.Empty;

	public string CanonicalUnitId { get; init; }  = string.Empty;

	public Dictionary<string, int> Dimension { get; init; } = new();
}