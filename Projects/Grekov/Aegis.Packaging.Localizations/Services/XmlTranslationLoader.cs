using System.Xml.Linq;
using Aegis.Packaging.Core.Entities;
using Aegis.Packaging.Core.Services;
using Aegis.Packaging.Localizations.Extensions;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Localizations;

internal sealed class XmlTranslationLoader
{
	private readonly IFileSystemService _fileSystem;
	private readonly ILogger<LocalizationService> _logger;
	private readonly ILocalizationRuntime _runtime;
	private readonly LocalizationLocaleService _locales;

	public XmlTranslationLoader(
		ILogger<LocalizationService> logger,
		IFileSystemService fileSystem,
		ILocalizationRuntime runtime,
		LocalizationLocaleService locales)
	{
		_fileSystem = fileSystem;
		_logger = logger;
		_runtime = runtime;
		_locales = locales;
	}

	public bool TryLoadXmlTranslations(PackageInstance package, List<ILocalizationTranslation> list, PackageConflictRegistry packageConflicts)
	{
		var xmlRoot = _locales.ResolveLocalizationXmlRoot(package);
		if (string.IsNullOrWhiteSpace(xmlRoot))
			return false;

		var requestedLocales = _locales.ResolveRequestedLocales(package);
		var files = _fileSystem.CollectFilesRecursive(
			xmlRoot,
			static name => name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));

		if (files.Count == 0)
			return false;

		var messagesByLocale = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

		foreach (var path in files)
		{
			string? text;
			try
			{
				text = _runtime.ReadAllText(path);
			}
			catch (Exception ex)
			{
				_logger.LocalizationXmlReadFailed(ex, path, package.Id);
				continue;
			}

			if (string.IsNullOrWhiteSpace(text))
				continue;

			XDocument doc;
			
			try
			{
				doc = XDocument.Parse(text, LoadOptions.None);
			}
			catch (Exception ex)
			{
				_logger.LocalizationXmlParseFailed(ex, path, package.Id);
				continue;
			}

			var root = doc.Root;
			if (root == null)
				continue;

			var locale = _locales.NormalizeLocale(root.Attribute("locale")?.Value);
			if (string.IsNullOrWhiteSpace(locale))
				locale = _locales.TryGetLocaleFromXmlPath(path);

			if (string.IsNullOrWhiteSpace(locale))
			{
				_logger.LocalizationXmlHasNoLocale(path, package.Id);
				continue;
			}

			if (requestedLocales.Count != 0 && !requestedLocales.Contains(locale))
				continue;

			if (!messagesByLocale.TryGetValue(locale, out var localeMessages))
				messagesByLocale[locale] = localeMessages = new Dictionary<string, string>(StringComparer.Ordinal);

			foreach (var element in root.Descendants())
			{
				var elementLocale = _locales.NormalizeLocale(element.Attribute("locale")?.Value);
				if (!string.IsNullOrWhiteSpace(elementLocale) &&
				    !string.Equals(elementLocale, locale, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				var key = element.Attribute("key")?.Value?.Trim();
				if (string.IsNullOrWhiteSpace(key))
					continue;

				var form = element.Attribute("form")?.Value?.Trim();
				if (!string.IsNullOrWhiteSpace(form))
					key = $"{key}.{form.ToLowerInvariant()}";

				var value = element.Value;
				if (string.IsNullOrWhiteSpace(value))
					continue;

				localeMessages[key] = value;
			}
		}

		if (messagesByLocale.Count == 0)
			return false;

		var fallbackLocale = _locales
			.ExpandLocaleCandidates(_locales.GetPackageFallbackLocale(package))
			.FirstOrDefault(messagesByLocale.ContainsKey);

		if (!string.IsNullOrWhiteSpace(fallbackLocale) &&
		    messagesByLocale.TryGetValue(fallbackLocale, out var fallbackMessages))
		{
			foreach (var (locale, localeMessages) in messagesByLocale)
			{
				if (string.Equals(locale, fallbackLocale, StringComparison.OrdinalIgnoreCase))
					continue;

				foreach (var (key, value) in fallbackMessages)
					localeMessages.TryAdd(key, value);
			}
		}

		var translationsByLocale = new Dictionary<string, ILocalizationTranslation>(StringComparer.OrdinalIgnoreCase);

		foreach (var (locale, localeMessages) in messagesByLocale)
		{
			var translation = _runtime.CreateTranslation(locale);

			foreach (var (key, value) in localeMessages.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
				_runtime.AddMessage(translation, key, value);

			translationsByLocale[locale] = translation;
		}

		if (translationsByLocale.Count == 0)
			return false;

		foreach (var translation in translationsByLocale.Values)
		{
			var key = $"xml:{translation.Locale}:{package.Id}";
			packageConflicts.Register($"translation:{key}", package.Id);
			
			_runtime.AddTranslation(translation);
			list.Add(translation);
		}

		return true;
	}
}
