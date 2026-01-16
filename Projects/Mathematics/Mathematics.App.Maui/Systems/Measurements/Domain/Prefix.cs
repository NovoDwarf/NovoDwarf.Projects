namespace Mathematics.App.Maui.Systems.Measurements.Domain;


public sealed class Prefix
{
	public string Id { get; init; } = string.Empty;

	public int Power { get; init; }

	public string DisplayName { get; init; } = string.Empty;
}