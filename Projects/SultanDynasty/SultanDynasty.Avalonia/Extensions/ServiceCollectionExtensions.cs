using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Grekov.Localizations.Interfaces;
using Grekov.Packaging.Interfaces;
using Grekov.Readers.Json.Extensions;
using SultanDynasty.Avalonia.Services;
using SultanDynasty.Avalonia.ViewModels;
using SultanDynasty.Avalonia.Views;
using SultanDynasty.Simulation.Extensions;

namespace SultanDynasty.Avalonia.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSultanDynastyAvalonia(this IServiceCollection services)
	{
		return services
			.AddDynastySimulation()
			.Json()
			.AddLogging(static builder => builder.AddDebug())
			.AddSingleton<IPackageCatalog, AvaloniaPackageCatalog>()
			.AddSingleton<IPackageLoadOrderStore, AvaloniaPackageLoadOrderStore>()
			.AddSingleton<ILocalizationRuntime, AvaloniaLocalizationRuntime>()
			.AddSingleton<AvaloniaDataBootstrapper>()
			.AddSingleton<AvaloniaSimulationHost>()
			.AddSingleton<MainWindowViewModel>()
			.AddTransient<MainWindow>();
	}
}
