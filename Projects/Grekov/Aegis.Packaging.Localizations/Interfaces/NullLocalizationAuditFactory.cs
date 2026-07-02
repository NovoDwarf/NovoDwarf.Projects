namespace Aegis.Packaging.Localizations;

public sealed class NullLocalizationAuditFactory : ILocalizationAuditFactory
{
	public ILocalizationAudit Create(LocalizationServerState state) => new NullLocalizationAudit();

	private sealed class NullLocalizationAudit : ILocalizationAudit
	{
		public bool AuditEnabled => false;
		public void Clear() { }
		public void EnsureTreeHook() { }
		public void SetAuditEnabled(bool enabled) { }
		public IReadOnlyList<string> GetAuditPackageIds() => [];
		public void AuditExistingTree() { }
	}
}