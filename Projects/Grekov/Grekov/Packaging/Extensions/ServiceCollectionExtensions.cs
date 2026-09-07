using Grekov.Packaging.Interfaces;
using Grekov.Packaging.Services;
using Grekov.Packaging.Services.Conflicts;
using Grekov.Packaging.Services.Discovery;
using Grekov.Packaging.Services.Loading;
using Grekov.Packaging.Services.Reloading;
using Grekov.Packaging.Services.Runtime;
using Grekov.Packaging.Services.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Grekov.Packaging.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGrekovPackaging(this IServiceCollection services)
	{
		return services
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
	}
}
