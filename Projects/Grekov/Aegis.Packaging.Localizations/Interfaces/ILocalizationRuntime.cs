namespace Aegis.Packaging.Localizations;

public interface ILocalizationRuntime
{
	string GlobalizePath(string path);
	string GetCurrentLocale();

	bool DirectoryExists(string path);
	IReadOnlyList<LocalizationDirEntry> ListDirectory(string path);

	bool ResourceExists(string path);
	string? ReadAllText(string path);

	ILocalizationTranslation? LoadTranslation(string path);
	ILocalizationTranslation CreateTranslation(string locale);
	void AddMessage(ILocalizationTranslation translation, string key, string value);
	void AddTranslation(ILocalizationTranslation translation);
	void RemoveTranslation(ILocalizationTranslation translation);
	void SaveTranslation(ILocalizationTranslation translation, string path);

	void EnsureDirectory(string path);
}