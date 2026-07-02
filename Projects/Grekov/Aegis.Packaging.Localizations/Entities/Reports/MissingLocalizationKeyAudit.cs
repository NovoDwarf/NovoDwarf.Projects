namespace Aegis.Packaging.Localizations;

internal sealed class MissingLocalizationKeyAudit
{
	public string PackageId { get; set; } = string.Empty;
	public string Key { get; set; } = string.Empty;
	public string ScenePath { get; set; } = string.Empty;
	public List<MissingLocalizationOccurrence> Occurrences { get; set; } = [];
}

