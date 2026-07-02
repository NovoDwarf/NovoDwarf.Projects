using Aegis.Packaging.Core.Entities;

namespace Aegis.Packaging.Localizations;

internal sealed class LocalizationLocaleService
{
	private readonly IFileSystemService _fileSystem;
	private readonly ILocalizationRuntime _runtime;

	public LocalizationLocaleService(ILocalizationRuntime runtime, IFileSystemService fileSystem)
	{
		_fileSystem = fileSystem;
		_runtime = runtime;
	}

	public HashSet<string> ResolveRequestedLocales(PackageInstance package)
	{
		var locales = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		foreach (var candidate in ExpandLocaleCandidates(GetCurrentGameLocale()))
			locales.Add(candidate);

		foreach (var candidate in ExpandLocaleCandidates(GetPackageFallbackLocale(package)))
			locales.Add(candidate);

		return locales;
	}

	public string? ResolveLocalizationXmlRoot(PackageInstance package)
	{
		var root = PackageAlias.GetLocalizationsFolder(package.RootPath);
		
		return _fileSystem.HasAnyFiles(root, FileAlias.Xml) ? root : null;
	}

	public string GetCurrentGameLocale()
	{
		return _runtime.GetCurrentLocale().Trim();
	}

	public string? GetPackageFallbackLocale(PackageInstance package)
	{
		return (from locale in package.Meta.Locales where !string.IsNullOrWhiteSpace(locale) select locale.Trim()).FirstOrDefault();
	}

	public IEnumerable<string> ExpandLocaleCandidates(string? locale)
	{
		var normalized = NormalizeLocale(locale);
		if (string.IsNullOrWhiteSpace(normalized))
			yield break;

		yield return normalized;

		var separatorIndex = normalized.IndexOf('-');
		if (separatorIndex > 0)
			yield return normalized[..separatorIndex];
	}

	public string TryGetLocaleFromXmlPath(string path)
	{
		var fileName = Path.GetFileNameWithoutExtension(path);
		if (string.IsNullOrWhiteSpace(fileName))
			return string.Empty;

		var separatorIndex = fileName.LastIndexOf('.');
		return separatorIndex > 0 && separatorIndex < fileName.Length - 1
			? NormalizeLocale(fileName[(separatorIndex + 1)..])
			: NormalizeLocale(fileName);
	}

	public string NormalizeLocale(string? locale)
	{
		return string.IsNullOrWhiteSpace(locale)
			? string.Empty
			: locale.Trim().Replace('_', '-');
	}
}
