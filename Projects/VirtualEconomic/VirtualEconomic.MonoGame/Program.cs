using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualEconomic.MonoGame.Extensions;

namespace VirtualEconomic.MonoGame;

internal static class Program
{
	public static void Main(string[] args)
	{
		var services = new ServiceCollection();

		services.AddServices();
		
		using var provider = services.BuildServiceProvider();

		provider.GetRequiredService<VirtualEconomicGame>().Run();
	}
}