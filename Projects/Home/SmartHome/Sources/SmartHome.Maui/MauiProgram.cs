using CommunityToolkit.Maui;
using SmartHome.Maui.Pages;
using SmartHome.Maui.Services;
using SmartHome.Maui.ViewModels;

namespace SmartHome.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddHttpClient("smarthome");

        builder.Services.AddSingleton<AppSettings>();
        builder.Services.AddSingleton<HealthApiClient>();

#if IOS
        builder.Services.AddSingleton<IHealthDataProvider, Platforms.iOS.HealthKitProvider>();
#elif ANDROID
        builder.Services.AddSingleton<IHealthDataProvider, Platforms.Android.HealthConnectProvider>();
#endif

        builder.Services.AddSingleton<SmartHomeApiClient>();

        builder.Services.AddTransient<DashboardPage, DashboardViewModel>();
        builder.Services.AddTransient<SettingsPage, SettingsViewModel>();
        builder.Services.AddTransient<HomePage, HomeViewModel>();
        builder.Services.AddTransient<LightsPage, LightsViewModel>();
        builder.Services.AddTransient<HumidifiersPage, HumidifiersViewModel>();
        builder.Services.AddTransient<KettlesPage, KettlesViewModel>();
        builder.Services.AddTransient<MediaPage, MediaViewModel>();
        builder.Services.AddTransient<ScenesPage, ScenesViewModel>();
        builder.Services.AddTransient<SensorsPage, SensorsViewModel>();

#if DEBUG
        //builder.Logging.AddLogging();
#endif

        return builder.Build();
    }
}
