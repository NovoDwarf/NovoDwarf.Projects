namespace Aegis.Packaging.Localizations;

public interface ILocalizationAudit
{
	bool AuditEnabled { get; }
	void Clear();
	void EnsureTreeHook();
	void SetAuditEnabled(bool enabled);
	IReadOnlyList<string> GetAuditPackageIds();
	void AuditExistingTree();
}