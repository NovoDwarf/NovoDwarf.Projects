using SultanDynasty.Definitons;
using SultanDynasty.Instances;
using SultanDynasty.Simulation.Generators;

namespace SultanDynasty.Simulation.Services;

public sealed class CharacterFactory
{
	private readonly IServiceProvider _services;
	private readonly World _world;
	private readonly CharacterGenerationPipeline _pipeline;

	public CharacterFactory(
		World world,
		IServiceProvider services,
		CharacterGenerationPipeline pipeline)
	{
		_world = world;
		_services = services;
		_pipeline = pipeline;
	}

	public CharacterRecord Create(CharacterDef def)
	{
		var entity = _world.Entities.CreateEntity();

		var context = new CharacterBuilderContext
		{
			World = _world,
			Entity = entity,
			BaseDef = def,
			Random = _world.Random,
			Services = _services,
			Character = new Character()
		};

		_pipeline.Execute(context);

		var record = new CharacterRecord(entity, context.Character);

		_world.Characters.Register(record);

		return record;
	}
}
