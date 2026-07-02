using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Interfaces;
using Aegis.Packaging.Core.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Localizations;

internal sealed class LocalizationContent
{
	private readonly TranslationRegistry _registry;
	private readonly XmlTranslationLoader _xmlLoader;

	public LocalizationContent(
		ILogger<LocalizationService> logger,
		IFileSystemService fileSystem,
		IPackageFingerprintProvider fingerprints,
		ILocalizationRuntime runtime,
		LocalizationServerState state)
	{
		_ = logger;
		_ = fingerprints;

		var localeService = new LocalizationLocaleService(runtime, fileSystem);

		_registry = new TranslationRegistry(runtime, state);
		_xmlLoader = new XmlTranslationLoader(
			logger,
			fileSystem,
			runtime,
			localeService);
	}

	public void Clear()
	{
		_registry.Clear();
	}

	public List<ILocalizationTranslation> GetOrCreateTranslationList(string packageId)
	{
		return _registry.GetOrCreateTranslationList(packageId);
	}

	public void RemoveTranslations(IEnumerable<ILocalizationTranslation> translations)
	{
		_registry.RemoveTranslations(translations);
	}

	public void RebuildKnownKeys()
	{
		_registry.RebuildKnownKeys();
	}

	public bool TryLoadXmlTranslations(PackageInstance package, List<ILocalizationTranslation> list, PackageConflictRegistry packageConflicts)
	{
		return _xmlLoader.TryLoadXmlTranslations(package, list, packageConflicts);
	}
}
