using Grekov.Packaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Grekov.Localizations.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGrekovLocalizations(this IServiceCollection services)
	{
		services.AddSingleton<LocalizationServerState>();
		services.AddSingleton<TranslationRegistry>();
		services.AddSingleton<LocalizationService>();
		services.AddSingleton<IPackageContentLoader>(static provider => provider.GetRequiredService<LocalizationService>());

		return services;
	}
}
