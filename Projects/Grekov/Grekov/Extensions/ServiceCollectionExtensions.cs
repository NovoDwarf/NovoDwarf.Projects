using Microsoft.Extensions.DependencyInjection;
using Grekov.Assemblies.Interfaces;
using Grekov.Assemblies.Services;
using Grekov.Defaults;
using Grekov.Definitions.Interfaces;
using Grekov.Definitions.Services;
using Grekov.Localizations.Services;
using Grekov.Packaging.Interfaces;
using Grekov.Packaging.Services.Conflicts;
using Grekov.Packaging.Services.Discovery;
using Grekov.Packaging.Services.Loading;
using Grekov.Packaging.Services.Reloading;
using Grekov.Packaging.Services.Runtime;
using Grekov.Packaging.Services.Validation;
using NovoDwarf.FS;
using NovoDwarf.FS.Interfaces;

namespace Grekov.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGrekov(this IServiceCollection services)
	{
		services.AddSingleton<IFileSystemService, PhysicalFileSystemService>();
		services.AddSingleton<IPathService, PathService>();

		services.AddSingleton<IPackagePathResolver, DefaultPathResolver>();
		services.AddSingleton<IPackageContextFactory, DefaultContextFactory>();
		services.AddSingleton<AssemblyLocator>();
		services.AddSingleton<EntrypointResolver>();
		services.AddSingleton<EntrypointActivator>();
		services.AddSingleton<AssemblyService>();
		services.AddSingleton<IPackageContentLoader>(static provider => provider.GetRequiredService<AssemblyService>());
		
		services.AddSingleton<LocalizationServerState>();
		services.AddSingleton<TranslationRegistry>();
		services.AddSingleton<LocalizationService>();
		services.AddSingleton<IPackageContentLoader>(static provider => provider.GetRequiredService<LocalizationService>());
		
		services.AddSingleton<DefIndex>();
		services.AddSingleton<DefReaderRegistry>();
		services.AddSingleton<DefScanner>();
		services.AddSingleton<DefService>();
		services.AddSingleton<IDefCatalog>(static provider => provider.GetRequiredService<DefService>());
		services.AddSingleton<IPackageContentLoader>(static provider => provider.GetRequiredService<DefService>());
		
		services
			.AddSingleton<PackageServiceState>()
			.AddSingleton<PackageConflictRegistry>()
			.AddSingleton<PackageDiscover>()
			.AddSingleton<DiscoveryWorkflow>()
			.AddSingleton<PackageLoadOrderBuilder>()
			.AddSingleton<PackageLoaderPipeline>()
			.AddSingleton<PackageReloadTransaction>()
			.AddSingleton<SnapshotStore>()
			.AddSingleton<ApplyWorkflow>()
			.AddSingleton<PackageValidator>()
			.AddSingleton<PackageService>()
			.AddSingleton<IPackageRuntime>(static provider => provider.GetRequiredService<PackageService>());
		
		return services;
	}
}
