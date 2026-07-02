using Aegis.Packaging.Core.Services;
using Aegis.Packaging.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aegis.Packaging.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddPackagingCore()
		{
			return services
				.AddSingleton<PackageConflictRegistry>()
				.AddSingleton<PackageDiscover>()
				.AddSingleton<PackageLoadExecutionService>()
				.AddSingleton<PackageService>()
				.AddSingleton<PackageValidator>();
		}
	}
}
