using Grekov.Extensions;
using Grekov.Localizations;
using Grekov.Packaging.Interfaces;
using Grekov.Readers.Json.Extensions;
using Grekov.Readers.Xml.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Grekov.Example;

public static class Program
{
	public static void Main(string[] args)
	{
		var packagesRoot = Path.Combine(AppContext.BaseDirectory, "Packages");
		
		Directory.CreateDirectory(packagesRoot);

		var samplePackageRoot = Path.Combine(packagesRoot, "Sample.Localization");
		var defsRoot = Path.Combine(samplePackageRoot, "Defs");
		
		Directory.CreateDirectory(defsRoot);

		File.WriteAllText(Path.Combine(defsRoot, "localization.json"),
			"""
			{
			  "translations": [
			    {
			      "LocalizedString": {
			        "id": "example/hello/en",
			        "locale": "en",
			        "key": "example.hello",
			        "value": "Hello from Grekov"
			      }
			    },
			    {
			      "LocalizedString": {
			        "id": "example/hello/ru",
			        "locale": "ru",
			        "key": "example.hello",
			        "value": "Привет из Grekov"
			      }
			    }
			  ]
			}
			""");

		var services = new ServiceCollection();

		services.AddSingleton<IGameVersionProvider, ExampleGameVersionProvider>();
		services.AddSingleton<IPackageLoadOrderStore, ExampleLoadOrderStore>();
		services.AddSingleton<IPackageFingerprintProvider, ExampleFingerprintProvider>();
		services.AddSingleton<IPackageCatalog>(_ => new ExamplePackageCatalog(samplePackageRoot));
		services.AddSingleton<ILocalizationRuntime, ExampleLocalizationRuntime>();

		services
			.AddGrekov()
			.AddReader()
			.Json()
			.Xml();

		var provider = services.BuildServiceProvider();
		var runtime = provider.GetRequiredService<IPackageRuntime>();

		runtime.LoadAll();

		var localization = (ExampleLocalizationRuntime)provider.GetRequiredService<ILocalizationRuntime>();

		Console.WriteLine("Loaded translations:");
		foreach (var translation in localization.Translations)
		foreach (var key in translation.GetKeys())
			Console.WriteLine($"{translation.Locale}: {key} = {translation.Messages[key]}");
	}
}