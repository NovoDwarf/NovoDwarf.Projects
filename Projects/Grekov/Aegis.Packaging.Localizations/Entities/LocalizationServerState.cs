namespace Aegis.Packaging.Localizations;

public sealed class LocalizationServerState
{
	internal bool AuditEnabled { get; set; }
	internal bool TreeHooked { get; set; }

	internal Dictionary<string, List<ILocalizationTranslation>> LoadedByPackage { get; } = new(StringComparer.OrdinalIgnoreCase);
	internal Dictionary<string, string> PackageRoots { get; } = new(StringComparer.OrdinalIgnoreCase);

	internal HashSet<string> KnownKeys { get; } = new(StringComparer.Ordinal);
	internal HashSet<string> WarnedEntries { get; } = new(StringComparer.Ordinal);
	internal Dictionary<string, MissingLocalizationKeyAudit> MissingKeys { get; } = new(StringComparer.Ordinal);
}
