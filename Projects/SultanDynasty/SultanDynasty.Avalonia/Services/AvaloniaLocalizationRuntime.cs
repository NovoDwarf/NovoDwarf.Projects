using System;
using System.Collections.Generic;
using Grekov.Localizations.Interfaces;

namespace SultanDynasty.Avalonia.Services;

public sealed class AvaloniaLocalizationRuntime : ILocalizationRuntime
{
	private readonly Dictionary<string, Dictionary<string, string>> _translations = new(StringComparer.OrdinalIgnoreCase);

	public void Apply(string locale, string key, string value)
	{
		if (!_translations.TryGetValue(locale, out var translations))
		{
			translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			_translations[locale] = translations;
		}

		translations[key] = value;
	}

	public void RemovePackage(string packageId)
	{
		_translations.Clear();
	}
}
