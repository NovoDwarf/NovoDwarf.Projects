namespace Aegis.Packaging.Localizations;

public sealed class MissingLocalizationAuditReport
{
	public DateTime GeneratedAtUtc { get; set; }
	public bool AuditEnabled { get; set; }
	public int PackageCount { get; set; }
	public int KeyCount { get; set; }
	public List<MissingLocalizationPackageReport> Packages { get; set; } = [];
}
