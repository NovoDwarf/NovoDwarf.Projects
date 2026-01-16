using AiForms.Settings;
using Camera.MAUI;
using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Mathematics.App.Maui.Systems.Application.Extensions;
using Mathematics.App.Maui.Systems.Measurements.Services;
using MemoryToolkit.Maui;
using Plugin.Maui.Audio;
using SkiaSharp.Views.Maui.Controls.Hosting;
using ZXing.Net.Maui.Controls;

namespace Mathematics.App;

public static class MauiProgram
{
	public static IServiceProvider Provider { get; private set; } = null!;

	public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
	        .AddAudio()
	        .UseSkiaSharp()
	        .UseLiveCharts()
	        .UseMauiApp<Maui.App>()
	        .UseBarcodeReader()
	        .UseMauiCameraView()
	        .UseSettingsView(true)
	        .UseMauiCommunityToolkit(ConfigureToolkit)
	        .UseLeakDetection()
	        .ConfigureExceptions()
	        .ConfigureFonts(ConfigureFonts);

        var registry = MeasurementLoader.LoadFromRawAsync().Result;

        builder.Services.AddSingleton(registry);

        builder
	        .RegisterServices()
	        .RegisterViewModels()
	        .RegisterViewsWithViewModel()
	        .RegisterPages()
	        .RegisterViews();

        var app = builder.Build();

        StartupServices(app);

        return app;
    }

	private static void StartupServices(MauiApp app)
	{
		Provider = app.Services;
	}

    private static void ConfigureToolkit(Options toolkit)
    {

    }

    private static void ConfigureFonts(IFontCollection fonts)
    {
	    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
	    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    }
}