namespace Aegis.Packaging.Localizations;

internal sealed class MissingLocalizationOccurrence
{
	public string NodePath { get; set; } = string.Empty;
	public string Slot { get; set; } = string.Empty;
	public string Reason { get; set; } = string.Empty;
}

