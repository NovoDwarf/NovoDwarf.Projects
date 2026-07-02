using Grekov.Assemblies.Interfaces;
using Grekov.Assemblies.Services;
using Grekov.Packaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Grekov.Assemblies.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGrekovAssemblies(this IServiceCollection services)
	{
		services.AddSingleton<IPackageAssemblyPathResolver, DefaultPackageAssemblyPathResolver>();
		services.AddSingleton<IPackageContextFactory, DefaultPackageContextFactory>();
		services.AddSingleton<PackageAssemblyLocator>();
		services.AddSingleton<EntrypointResolver>();
		services.AddSingleton<EntrypointActivator>();
		services.AddSingleton<AssemblyService>();
		services.AddSingleton<IPackageContentLoader>(static provider => provider.GetRequiredService<AssemblyService>());

		return services;
	}
}
