namespace Mathematics.App.Maui.Systems.Measurements.Domain;

public sealed class Quantity
{
	public required string Id { get; init; }

	public required string DisplayName { get; init; }

	public required string CanonicalUnitId { get; init; }

	public Dimension Dimension { get; init; }
}