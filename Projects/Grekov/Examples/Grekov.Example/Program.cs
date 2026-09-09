using Grekov.Extensions;
using Grekov.Localizations;
using Grekov.Localizations.Entities;
using Grekov.Localizations.Interfaces;
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

		services.AddSingleton<IPackageLoadOrderStore, ExampleLoadOrderStore>();
		services.AddSingleton<IPackageCatalog>(_ => new ExamplePackageCatalog(samplePackageRoot));
		services.AddSingleton<ILocalizationRuntime, ExampleLocalizationRuntime>();

		services
			.AddGrekov()
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

internal sealed class ExampleLoadOrderStore : IPackageLoadOrderStore
{
	public IReadOnlyDictionary<string, bool> LoadEnabledOverrides()
	{
		return new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
	}
}

internal sealed class ExamplePackageCatalog : IPackageCatalog
{
	private readonly string _packageRoot;

	public ExamplePackageCatalog(string packageRoot)
	{
		_packageRoot = packageRoot;
	}

	public IEnumerable<string> GetPackageRoots()
	{
		yield return _packageRoot;
	}
}

internal sealed class ExampleLocalizationRuntime : ILocalizationRuntime
{
	private readonly Dictionary<string, Translation> _translations = new(StringComparer.OrdinalIgnoreCase);

	public IEnumerable<Translation> Translations => _translations.Values;

	public void Apply(string locale, string key, string value)
	{
		if (!_translations.TryGetValue(locale, out var translation))
		{
			translation = new Translation(locale);
			_translations[locale] = translation;
		}

		translation.Messages[key] = value;
	}

	public void RemovePackage(string packageId)
	{
	}
}
