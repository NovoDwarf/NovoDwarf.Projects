using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Application.Entities;
using NovoDwarf.Mathematics.App.Systems.Application.Services;

namespace NovoDwarf.Mathematics.App.ViewModels.Pages;

public sealed partial class SettingsViewModel : BaseViewModel
{
    private readonly IThemeService _themeService;
    private readonly ILocaleService _localeService;

    public SettingsViewModel(IThemeService themeService, ILocaleService localeService)
    {
        _themeService = themeService;
        _localeService = localeService;

        AvailableThemes = _themeService.Themes;
        AvailableLocales = _localeService.Locales;

        SelectedTheme = _themeService.Current;
        SelectedLocale = _localeService.Current;
    }

    [ObservableProperty]
    public partial AppTheme SelectedTheme { get; set; }

    [ObservableProperty]
    public partial LocaleItem? SelectedLocale { get; set; }

    public ObservableCollection<AppTheme> AvailableThemes { get; }

    public ObservableCollection<LocaleItem> AvailableLocales { get; }

    [RelayCommand]
    private void SetTheme(AppTheme theme) => _themeService.Set(theme);

    [RelayCommand]
    private void SetLocale(LocaleItem locale) => _localeService.Set(locale);
}