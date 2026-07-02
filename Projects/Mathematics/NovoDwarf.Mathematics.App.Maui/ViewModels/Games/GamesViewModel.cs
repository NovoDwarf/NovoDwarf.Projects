using System.Collections.ObjectModel;
using LocalizationResourceManager.Maui;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Resources.Localizations;
using NovoDwarf.Mathematics.App.Systems.Application.Services;
using NovoDwarf.Mathematics.App.ViewModels.Common;
using NovoDwarf.Mathematics.App.Views.Modules.Games;
using Utilities.Extensions;

namespace NovoDwarf.Mathematics.App.ViewModels.Games;

public class GamesViewModel : BaseViewModel
{
	private readonly ILocalizationResourceManager _manager;
	private readonly IThemeService _themeService;
	private readonly Dictionary<PreviewViewModel, string> _iconMappings = [];

	public GamesViewModel(ILocalizationResourceManager manager, IThemeService themeService)
	{
		_manager = manager;
		_themeService = themeService;

		Initialize();

		_themeService.ThemeChanged += OnThemeChanged;
	}

	public ObservableCollection<PreviewViewModel> Games { get; private set; } = [];
	
	private void Initialize()
    {
        var isDark = _themeService.IsDarkMode;

        var sensorData = new[]
        {
            ("Title_Accelerometer", typeof(SlidingTilePuzzlePage), "accelerometer"),
        };

        Games = CreateCollection(sensorData, isDark);
    }

    private ObservableCollection<PreviewViewModel> CreateCollection((string titleKey, Type viewType, string iconBaseName)[] data, bool isDark)
    {
        var collection = new ObservableCollection<PreviewViewModel>();

        foreach (var (titleKey, viewType, iconBaseName) in data)
        {
            var vm = new PreviewViewModel
            {
                Title = Components_Resources.ResourceManager.GetString(titleKey) ?? titleKey,
                View = viewType,
                Icon = GetIcon(iconBaseName, isDark)
            };

            _iconMappings[vm] = iconBaseName;

            collection.Add(vm);
        }

        return collection.Sort(i => i.Title);
    }

    private string GetIcon(string baseName, bool isDark)
    {
        return isDark ? $"games_{baseName}_dark.png" : $"games_{baseName}_light.png";
    }

    private void OnThemeChanged(object? sender, AppTheme newTheme)
    {
        var isDark = newTheme == AppTheme.Dark;

        foreach (var (vm, icon) in _iconMappings)
        {
            vm.Icon = GetIcon(icon, isDark);
        }
    }

    public override void Cleanup()
    {
        _iconMappings?.Clear();
        _themeService.ThemeChanged -= OnThemeChanged;
    }
}