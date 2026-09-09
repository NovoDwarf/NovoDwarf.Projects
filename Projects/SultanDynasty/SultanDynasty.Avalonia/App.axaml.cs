using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SultanDynasty.Avalonia.Services;
using SultanDynasty.Avalonia.Views;
using SultanDynasty.Simulation;

namespace SultanDynasty.Avalonia;

public partial class App : Application
{
	public static IServiceProvider Services { get; set; } = null!;
	
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		Services.GetRequiredService<AvaloniaDataBootstrapper>().Load();
		
		var bootstrapper = Services.GetRequiredService<SimulationBootstrapper>();
		var host = Services.GetRequiredService<AvaloniaSimulationHost>();
		
		bootstrapper.Initialize();
		host.Start();
		
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = Services.GetRequiredService<MainWindow>();
			desktop.Exit += (_, _) =>
			{
				host.Dispose();
				
				if (Services is IDisposable disposable)
					disposable.Dispose();
			};
		}
		
		base.OnFrameworkInitializationCompleted();
	}
}
