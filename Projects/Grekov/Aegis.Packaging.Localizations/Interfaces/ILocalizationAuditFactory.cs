namespace Aegis.Packaging.Localizations;

public interface ILocalizationAuditFactory
{
	public ILocalizationAudit Create(LocalizationServerState state);
}