using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Maui;
using LocalizationResourceManager.Maui;
using Mathematics.App.Maui.Resources.Localizations;
using Mathematics.App.Maui.Services.Views;
using Mathematics.App.Maui.Systems.Application.Handlers;
using Mathematics.App.Maui.Systems.Application.Services;
using Mathematics.App.Maui.Systems.Measurements.Interfaces;
using Mathematics.App.Maui.Systems.Measurements.Services;
using Mathematics.App.Maui.Systems.Sensors.Processing;
using Mathematics.App.Maui.Systems.Sensors.Services;
using Mathematics.App.Maui.UI.ViewModels.Algorithms;
using Mathematics.App.Maui.UI.ViewModels.Distributions;
using Mathematics.App.Maui.UI.ViewModels.Pages;
using Mathematics.App.Maui.UI.ViewModels.Utilities;
using Mathematics.App.Maui.UI.Views.Common;
using Mathematics.App.Maui.UI.Views.Distributions;
using Mathematics.App.Maui.UI.Views.Games;
using Mathematics.App.Maui.UI.Views.Graphics;
using Mathematics.App.Maui.UI.Views.Graphics.Drawings;
using Mathematics.App.Maui.UI.Views.Graphics.Fractals;
using Mathematics.App.Maui.UI.Views.Pages;
using Mathematics.App.Maui.UI.Views.Pages.Algorithms;
using Mathematics.App.Maui.UI.Views.Utilities;
using Mathematics.App.Maui.UI.Views.Utilities.Organizers;
using Mathematics.App.Systems.Algorithms.Services;
using Mathematics.App.Systems.Hosting.Interfaces;
using Mathematics.App.ViewModels.Utilities;
using Mathematics.App.Views;
using Mathematics.App.Views.Common;
using Mathematics.Core.Resources;
using Mathematics.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using CalculatorService = Mathematics.App.Maui.Systems.Sensors.Services.CalculatorService;
using NotesViewModel = Mathematics.App.Maui.UI.ViewModels.Utilities.NotesViewModel;

namespace Mathematics.App.Maui.Systems.Application.Extensions;

public static class MauiExtensions
{
	extension(MauiAppBuilder builder)
	{
		public MauiAppBuilder RegisterServices()
		{
			builder.Logging.AddDebug();
			builder.Services.AddSerilog(ConfigureLogger, false, true);

			builder.UseLocalizationResourceManager(settings =>
			{
				settings.SupportNameWithDots();
				settings.AddResource(Components_Resources.ResourceManager);
				settings.AddResource(Base_Resources.ResourceManager);
				settings.AddResource(Distributions_Resources.ResourceManager);
				settings.AddResource(Numerical_Resources.ResourceManager);
				settings.AddResource(Measurement_Resources.ResourceManager);
				settings.AddResource(Parameters_Resources.ResourceManager);
				settings.RestoreLatestCulture(true);
				settings.SuppressTextNotFoundException();
			});

			builder.Services.AddDistributions();

			builder.Services.AddSingleton<IMeasurementRegistry, InMemoryMeasurementRegistry>();

			builder.Services.AddSingleton<IThemeService, ThemeService>();
			builder.Services.AddSingleton<ILocaleService, LocaleService>();
			builder.Services.AddSingleton<ActivityService>();

			builder.Services.AddSingleton<ParameterService>();
			builder.Services.AddSingleton<DistributionService>();

			builder.Services.AddSingleton<AccelerometerProcessingService>();
			builder.Services.AddSingleton<BarometerProcessingService>();

			builder.Services.AddSingleton<AccelerometerService>();
			builder.Services.AddSingleton<MagnetometerService>();
			builder.Services.AddSingleton<BarometerService>();
			builder.Services.AddSingleton<FlashlightService>();
			builder.Services.AddSingleton<RoundingService>();
			builder.Services.AddSingleton<CalculatorService>();

			return builder;
		}

		public MauiAppBuilder RegisterViewModels()
		{
			builder.Services.AddTransient(typeof(DistributionViewModel<>));

			return builder;
		}

		public MauiAppBuilder RegisterViewsWithViewModel()
		{
			// Light
			builder.Services.AddSingleton<MainPage, MainViewModel>();
			builder.Services.AddSingleton<SettingsPage, SettingsViewModel>();
			builder.Services.AddSingleton<DistributionPage, DistributionsViewModel>();
			builder.Services.AddSingleton<NotesPage, NotesViewModel>();
			builder.Services.AddSingleton<ConverterPage, ConverterViewModel>();
			builder.Services.AddSingleton<CashCalculatorPage, CashCalculatorViewModel>();
			builder.Services.AddSingleton<CodeGeneratorPage, CodeGeneratorViewModel>();
			builder.Services.AddSingleton<SlidingTilePuzzlePage, SlidingTilePuzzleViewModel>();

			// Heavy
			builder.Services.AddTransient<FlashlightPage, FlashlightViewModel>();
			builder.Services.AddTransient<CalculatorPage, CalculatorViewModel>();
			builder.Services.AddTransient<LevelPage, LevelViewModel>();
			builder.Services.AddTransient<StopwatchPage, StopwatchViewModel>();
			builder.Services.AddTransient<TimerPage, TimerViewModel>();
			builder.Services.AddTransient<AccelerometerPage, AccelerometerViewModel>();
			builder.Services.AddTransient<BarometerPage, BarometerViewModel>();
			builder.Services.AddTransient<SpeedometerPage, SpeedometerViewModel>();
			builder.Services.AddTransient<CompassPage, CompassViewModel>();
			builder.Services.AddTransient<CodeScannerPage, CodeScannerViewModel>();
			builder.Services.AddTransient<MirrorPage, MirrorViewModel>();
			builder.Services.AddTransient<SynthPage, SynthViewModel>();
			builder.Services.AddTransient<SoundGeneratorPage, SoundGeneratorViewModel>();
			builder.Services.AddTransient<UtilityPage, UtilityViewModel>();
			builder.Services.AddTransient<DeviceInfoPage, DeviceInfoViewModel>();

			return builder;
		}

		public MauiAppBuilder RegisterViews()
		{
			builder.Services.AddTransient<PreviewTableView>();
			builder.Services.AddTransient<PreviewSquareView>();

			return builder;
		}

		public MauiAppBuilder RegisterPages()
		{
			var lightPageTypes = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t is { IsClass: true, IsAbstract: false } && typeof(ILightPage).IsAssignableFrom(t));

			foreach (var type in lightPageTypes) 
				builder.Services.AddSingleton(type);

			// Heavy
			builder.Services.AddTransient<LineDrawPage>();
			builder.Services.AddTransient<CirclesDrawPage>();
			builder.Services.AddTransient<FillDrawPage>();
			builder.Services.AddTransient<TruncationDrawPage>();
			builder.Services.AddTransient<NoisesPage>();
			builder.Services.AddTransient<FractalPage>();
			builder.Services.AddTransient<KochCurvePage>();

			// Conditional
			builder.Services.AddTransient<DistributionPage>();

			return builder;
		}

		public MauiAppBuilder ConfigureExceptions()
		{
			GlobalExceptionHandler.UnhandledException += GlobalExceptionHandler_UnhandledException;

			return builder;

			void GlobalExceptionHandler_UnhandledException(object sender, UnhandledExceptionEventArgs e)
			{
				var exception = e.ExceptionObject as Exception;

				Shell.Current.DisplayAlertAsync("Произошло неперехваченное исключение", exception?.Message, "Выход");

				Log.Error(exception, "Global exception caught");
				Debug.Print(exception?.Message);
				Console.WriteLine(exception?.Message);
			}
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