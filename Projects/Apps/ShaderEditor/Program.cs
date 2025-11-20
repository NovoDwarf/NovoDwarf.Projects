using Autofac;
using Autofac.Extensions.DependencyInjection;
using Messager.NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Enrichers.CallerInfo;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using ShaderEditor.Backgrounds;
using ShaderEditor.Services;

namespace ShaderEditor;

internal static class Program
{
	private static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		var provider = new AutofacServiceProviderFactory();

		builder.ConfigureContainer(provider, Register);

		builder.Services.AddSerilog(ConfigureLogger);
		builder.Services.AddHostedService<EditorBackgroundService>();

		builder.Services.AddActivatedSingleton<DX11Service>();
		builder.Services.AddActivatedSingleton<ImGuiService>();
		builder.Services.AddActivatedSingleton<EditorService>();

		using var app = builder.Build();

		await app.RunAsync();
	}

	private static void ConfigureLogger(LoggerConfiguration configuration)
	{
		const string outputTemplate =
			"[{Timestamp:HH:mm:ss} {Level:u3}] [{Namespace}] [{Method}] {Message:lj}{NewLine}{Exception}";

		var levelSwitcher = new LoggingLevelSwitch
		{
			MinimumLevel = LogEventLevel.Verbose
		};

		SelfLog.Enable(Console.WriteLine);

		configuration
			.MinimumLevel.ControlledBy(levelSwitcher)
			.MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
			.Enrich.FromLogContext()
			.Enrich.WithCallerInfo(
				true,
				"ShaderEditor",
				startingAssembly: "ShaderEditor")
			.WriteTo.Console(
				outputTemplate: outputTemplate,
				theme: AnsiConsoleTheme.Code,
				levelSwitch: levelSwitcher);
	}

	private static void Register(ContainerBuilder containerBuilder)
	{
		containerBuilder.AddMessager();
	}
}