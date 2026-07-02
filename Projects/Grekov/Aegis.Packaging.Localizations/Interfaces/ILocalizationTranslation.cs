namespace Aegis.Packaging.Localizations;

public interface ILocalizationTranslation
{
	public string Locale { get; }
	public IEnumerable<string> GetKeys();
}