using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using LocalizationResourceManager.Maui;
using Mathematics.App.Maui.Factories;
using Mathematics.App.Maui.Models;
using Mathematics.App.Maui.Resources.Localizations;
using Mathematics.App.Maui.Services;
using Mathematics.App.Maui.UI.ViewModels;
using Mathematics.App.Maui.UI.ViewModels.Common;
using Mathematics.App.Maui.UI.Views.Common;
using Mathematics.App.Maui.UI.Views.Pages;
using Mathematics.Core.Resources;
using SkiaSharp.Views.Maui.Controls.Hosting; 
using MemoryToolkit.Maui;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Mathematics.App.Maui;

public static class MauiExtensions 
{
	public static MauiAppBuilder RegisterFactories(this MauiAppBuilder builder)
	{
		builder.Logging.AddDebug();
		builder.Services.AddSerilog(ConfigureLogger, false, true);
		
		builder.UseLocalizationResourceManager(settings =>
            {
                settings.AddResource(Components_Resources.ResourceManager);
                settings.AddResource(Base_Resources.ResourceManager);
				settings.AddResource(Distributions_Resources.ResourceManager);
				settings.AddResource(Functions_Resources.ResourceManager);
				settings.SupportNameWithDots();
                settings.RestoreLatestCulture(true);
                settings.SuppressTextNotFoundException();
            });
		
		builder.Services.AddSingleton<DistributionPageFactory>();
		builder.Services.AddSingleton<DistributionPreViewModelFactory>();
		builder.Services.AddSingleton<IPageFactory, PageFactory>();
		
		builder.Services.AddSingleton<NavigationService>();

		return builder;
	}
	
	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<PreviewViewModel>();
		builder.Services.AddSingleton<DistributionsViewModel>();
		
		return builder;
	}
	
	public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<PreviewView>();

		return builder;
	}

	public static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<AboutPage>();
		builder.Services.AddSingleton<SettingsPage>();
		builder.Services.AddSingleton<AlgorithmsPage>();
		
		builder.Services.AddSingleton<DistributionsPage>();
		builder.Services.AddSingleton<CompressionsPage>();
		builder.Services.AddSingleton<CryptographyPage>();
		builder.Services.AddSingleton<FunctionsPage>();
		builder.Services.AddSingleton<GraphicsPage>();
		builder.Services.AddSingleton<RandomsPage>();
		builder.Services.AddSingleton<SortingsPage>();
		
		return builder;
	}

	public static MauiAppBuilder ConfigureExceptions(this MauiAppBuilder builder)
	{
		GlobalExceptionHandler.UnhandledException += GlobalExceptionHandler_UnhandledException;

		return builder;

		void GlobalExceptionHandler_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = e.ExceptionObject as Exception;
			using var services = builder.Services.BuildServiceProvider();
			var logger = services.GetRequiredService<ILogger<Page>>();
			
			logger.LogError(exception, "Произошла непредвиденная ошибка");
		}
	}
	
	private static void ConfigureLogger(LoggerConfiguration configuration)
	{
		const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{Namespace}] [{Method}] {Message:lj}{NewLine}{Exception}";
		
		SelfLog.Enable(Console.WriteLine);
		
		configuration
			.MinimumLevel.Verbose()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
			.Enrich.FromLogContext()
			.WriteTo.Debug(outputTemplate: outputTemplate)
			.WriteTo.Console(outputTemplate: outputTemplate, theme: AnsiConsoleTheme.Code);
	}
}

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
	        .UseSkiaSharp() 
	        .UseLiveCharts()
	        .UseMauiApp<App>()
	        .UseMauiCommunityToolkit(ConfigureToolkit)
	        .UseLeakDetection()
	        .ConfigureExceptions()
	        .ConfigureFonts(ConfigureFonts);
        
        builder
	        .RegisterFactories()
	        .RegisterViewModels()
	        .RegisterPages()
	        .RegisterViews();
        
        return builder.Build();
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