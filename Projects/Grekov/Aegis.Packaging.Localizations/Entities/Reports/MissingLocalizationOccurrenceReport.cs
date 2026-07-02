namespace Aegis.Packaging.Localizations;

public sealed class MissingLocalizationOccurrenceReport
{
	public string NodePath { get; set; } = string.Empty;
	public string Slot { get; set; } = string.Empty;
	public string Reason { get; set; } = string.Empty;
}
