using Grekov.Definitions.Interfaces;
using Grekov.Readers.Json.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Grekov.Readers.Json.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection Json(this IServiceCollection services)
	{
		return services.AddSingleton<IDefinitionFormatReader, DefinitionJsonReader>();
	}

	public static IServiceCollection AddGrekovJsonDefinitions(this IServiceCollection services)
	{
		return services.Json();
	}
}
