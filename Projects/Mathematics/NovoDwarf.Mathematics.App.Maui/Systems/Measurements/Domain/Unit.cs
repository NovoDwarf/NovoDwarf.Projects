namespace NovoDwarf.Mathematics.App.Systems.Measurements.Domain;

public sealed class Unit
{
	public required string Id { get; init; }
	public required string DisplayName { get; init; }

	public required string QuantityId { get; init; }
	public required string SystemId { get; init; }

	public bool AllowPrefixes { get; init; }

	public double Factor { get; init; }
	public double Offset { get; init; }

	public Prefix? Prefix { get; init; }
}