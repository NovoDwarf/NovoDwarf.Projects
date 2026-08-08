using Grekov.Definitions.Interfaces;
using Grekov.Definitions.Services;
using Grekov.Packaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Grekov.Definitions.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGrekovDefinitions(this IServiceCollection services)
	{
		services.AddSingleton<DefIndex>();
		services.AddSingleton<DefScanner>();
		services.AddSingleton<DefService>();
		services.AddSingleton<IDefCatalog>(static provider => provider.GetRequiredService<DefService>());
		services.AddSingleton<IPackageContentLoader>(static provider => provider.GetRequiredService<DefService>());

		return services;
	}
}
