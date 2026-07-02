using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Maui;
using LocalizationResourceManager.Maui;
using Mathematics.Core.Resources;
using Mathematics.DependencyInjection;
using Microsoft.Extensions.Logging;
using NovoDwarf.Mathematics.App.Resources.Localizations;
using NovoDwarf.Mathematics.App.Systems.Algorithms.Services;
using NovoDwarf.Mathematics.App.Systems.Application.Handlers;
using NovoDwarf.Mathematics.App.Systems.Application.Services;
using NovoDwarf.Mathematics.App.Systems.Hosting.Interfaces;
using NovoDwarf.Mathematics.App.Systems.Measurements.Interfaces;
using NovoDwarf.Mathematics.App.Systems.Measurements.Services;
using NovoDwarf.Mathematics.App.Systems.Sensors.Processing;
using NovoDwarf.Mathematics.App.Systems.Sensors.Services;
using NovoDwarf.Mathematics.App.ViewModels.Pages;
using NovoDwarf.Mathematics.App.ViewModels.Probability;
using NovoDwarf.Mathematics.App.ViewModels.Utilities;
using NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;
using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;
using NovoDwarf.Mathematics.App.Views;
using NovoDwarf.Mathematics.App.Views.Common;
using NovoDwarf.Mathematics.App.Views.Modules.Games;
using NovoDwarf.Mathematics.App.Views.Modules.Graphics;
using NovoDwarf.Mathematics.App.Views.Modules.Graphics.Drawings;
using NovoDwarf.Mathematics.App.Views.Modules.Graphics.Fractals;
using NovoDwarf.Mathematics.App.Views.Modules.Probability.Distributions;
using NovoDwarf.Mathematics.App.Views.Modules.Utilities.Organizers;
using NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;
using NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using AccelerometerViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors.AccelerometerViewModel;
using BarometerViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors.BarometerViewModel;
using CalculatorService = NovoDwarf.Mathematics.App.Systems.Algorithms.Services.CalculatorService;
using CalculatorViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools.CalculatorViewModel;
using CashCalculatorViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools.CashCalculatorViewModel;
using CodeGeneratorViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools.CodeGeneratorViewModel;
using CodeScannerViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools.CodeScannerViewModel;
using CompassViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors.CompassViewModel;
using ConverterViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers.ConverterViewModel;
using FlashlightViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors.FlashlightViewModel;
using LevelViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools.LevelViewModel;
using NotesViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers.NotesViewModel;
using SettingsViewModel = NovoDwarf.Mathematics.App.ViewModels.Pages.SettingsViewModel;
using SoundGeneratorViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools.SoundGeneratorViewModel;
using SpeedometerViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors.SpeedometerViewModel;
using StopwatchViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers.StopwatchViewModel;
using TimerViewModel = NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers.TimerViewModel;
using UtilityPage = NovoDwarf.Mathematics.App.Views.Modules.UtilityPage;

namespace NovoDwarf.Mathematics.App.Systems.Application.Extensions;

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
			builder.Services.AddTransient(typeof(global::NovoDwarf.Mathematics.App.ViewModels.Probability.Distributions.DistributionViewModel<>));

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