using Microsoft.Extensions.DependencyInjection;
using Grekov.Assemblies.Extensions;
using Grekov.Definitions.Extensions;
using Grekov.Localizations.Extensions;
using Grekov.Packaging.Extensions;
using NovoDwarf.FS;
using NovoDwarf.FS.Interfaces;

namespace Grekov.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGrekov(this IServiceCollection services)
	{
		services.AddSingleton<IFileSystemService, PhysicalFileSystemService>();
		services.AddSingleton<IPathService, PathService>();

		services
			.AddGrekovPackaging()
			.AddGrekovDefinitions()
			.AddGrekovLocalizations()
			.AddGrekovAssemblies();

		return services;
	}
}
