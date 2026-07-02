namespace Aegis.Packaging.Localizations;

public sealed class MissingLocalizationKeyReport
{
	public string Key { get; set; } = string.Empty;
	public string ScenePath { get; set; } = string.Empty;
	public List<MissingLocalizationOccurrenceReport> Occurrences { get; set; } = [];
}
