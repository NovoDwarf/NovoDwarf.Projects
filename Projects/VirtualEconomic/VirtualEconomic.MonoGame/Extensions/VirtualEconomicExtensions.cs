using Grekov.Extensions;
using Grekov.Readers.Json.Extensions;
using Komissar.Extensions;
using Microsoft.Extensions.DependencyInjection;
using VirtualEconomic.Systems;
using VirtualEconomic.Systems.Populations;

namespace VirtualEconomic.MonoGame.Extensions;

public static class VirtualEconomicExtensions
{
	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddSingleton<IGameUI, GameUI>();
		services.AddSingleton<VirtualEconomicGame>();
		
		services.AddGrekov().Json();
		services.AddKomissar<EconomyState>();
		
		return services;
	}
}