namespace Aegis.Packaging.Localizations;

public sealed class MissingLocalizationPackageReport
{
	public string PackageId { get; set; } = string.Empty;
	public int EntryCount { get; set; }
	public List<MissingLocalizationKeyReport> Entries { get; set; } = [];
}
