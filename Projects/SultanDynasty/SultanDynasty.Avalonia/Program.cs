using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using SultanDynasty.Avalonia.Extensions;
using System;

namespace SultanDynasty.Avalonia;

sealed class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		App.Services = new ServiceCollection()
			.AddSultanDynastyAvalonia()
			.BuildServiceProvider();
		
		BuildAvaloniaApp()
			.StartWithClassicDesktopLifetime(args);
	}

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
		             .UsePlatformDetect()
		             .WithInterFont()
		             .LogToTrace();
}
