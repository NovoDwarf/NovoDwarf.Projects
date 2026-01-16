using System.Collections.ObjectModel;
using System.Globalization;
using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.Systems.Application.Entities;

namespace Mathematics.App.Maui.Systems.Application.Services;

public interface ILocaleService
{
	public LocaleItem Current { get; }

	public ObservableCollection<LocaleItem> Locales { get; }

	public void Set(LocaleItem locale);
}

public class LocaleService : ILocaleService
{
	public LocaleService()
	{
		var savedLocale = Preferences.Get(PreferenceKeys.Locale, CultureInfo.CurrentUICulture.Name);

		Locales =
		[
			new LocaleItem("en-US", "English"),
			new LocaleItem("ru-RU", "Русский")
		];

		Current = Locales.FirstOrDefault(l => l.CultureCode == savedLocale) ?? Locales[0];
	}

	public LocaleItem Current { get; private set; }

	public ObservableCollection<LocaleItem> Locales { get; private set; }

	public void Set(LocaleItem locale)
	{
		var culture = new CultureInfo(locale.CultureCode);

		CultureInfo.CurrentCulture = culture;
		CultureInfo.CurrentUICulture = culture;

		Thread.CurrentThread.CurrentCulture = culture;
		Thread.CurrentThread.CurrentUICulture = culture;
	}

	private static void Save(LocaleItem locale) => Preferences.Set(PreferenceKeys.Locale, locale.CultureCode);
}