using System.Collections.ObjectModel;
using Mathematics.App.Maui.Systems.Application.Constants;

namespace Mathematics.App.Maui.Systems.Application.Services;

public interface IThemeService
{
    event EventHandler<AppTheme>? ThemeChanged;

    AppTheme Current { get; }
    ObservableCollection<AppTheme> Themes { get; }
    bool IsDarkMode { get; }
    bool IsLightMode { get; }

    void Set(AppTheme theme);
}

public class ThemeService : IThemeService
{
    public event EventHandler<AppTheme>? ThemeChanged;
    
    public ThemeService()
    {
        var savedTheme = Preferences.Get(PreferenceKeys.Theme, nameof(AppTheme.Unspecified));
        
        Set(Enum.TryParse(savedTheme, out AppTheme theme) ? theme : AppTheme.Unspecified);
        
        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += (sender, args) =>
        {
            ThemeChanged?.Invoke(this, args.RequestedTheme);
        };
    }

    public AppTheme Current => Microsoft.Maui.Controls.Application.Current!.RequestedTheme;

    public ObservableCollection<AppTheme> Themes => [AppTheme.Unspecified, AppTheme.Light, AppTheme.Dark];

    public bool IsDarkMode => Current == AppTheme.Dark;

    public bool IsLightMode => Current == AppTheme.Light;

    public void Set(AppTheme theme)
    {
        Save(theme);
        
        Microsoft.Maui.Controls.Application.Current!.UserAppTheme = theme;
        
        ThemeChanged?.Invoke(this, theme);
    }
    
    private static void Save(AppTheme theme) => Preferences.Set(PreferenceKeys.Theme, theme.ToString());
}
