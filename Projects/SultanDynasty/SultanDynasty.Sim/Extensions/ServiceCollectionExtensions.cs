using Grekov.Extensions;
using Komissar;
using Komissar.Core.Enums;
using Komissar.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SultanDynasty.Simulation.Generators;
using SultanDynasty.Simulation.Generators.Interfaces;
using SultanDynasty.Simulation.Services;
using SultanDynasty.Simulation.Systems;

namespace SultanDynasty.Simulation.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDynastySimulation(this IServiceCollection services)
	{
		services.AddEngineCore();
		services.AddWorld();
		services.AddGrekov();

		services.Scan(scan 
			=> scan.FromAssemblyOf<ICharacterGenerationStep>()
			       .AddClasses(classes => classes.AssignableTo<ICharacterGenerationStep>())
			       .AsImplementedInterfaces()
			       .WithSingletonLifetime());
		
		services.AddSingleton<CharacterGenerationPipeline>();
		services.AddSingleton<DefaultCharacterDefinitionCatalog>();
		services.AddSingleton<CharacterRegistry>();
		services.AddSingleton<CharacterFactory>();
		
		return services;
	}

	public static IServiceCollection AddWorld(this IServiceCollection services)
	{
		services.AddSingleton<RuleEngine>();
		services.AddSingleton<World>();

		return services;
	}
	
	public static IServiceCollection AddEngineCore(this IServiceCollection services)
	{
		services.AddKomissar<World>(
			static provider => provider.GetRequiredService<World>(),
			options =>
			{
				options.Mode = SimulationMode.Turn;
				options.MaxDegreeOfParallelism = Environment.ProcessorCount;
			},
			timeScale => timeScale.Speed = 60);
		services.AddKomissarSystem<World, NeedsSystem>();
		services.AddSingleton<SimulationBootstrapper>();

		return services;
	}
}
