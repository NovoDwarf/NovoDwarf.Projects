using SultanDynasty.Definitons;
using SultanDynasty.Simulation.Services;

namespace SultanDynasty.Simulation;

public sealed class SimulationBootstrapper
{
	private readonly CharacterFactory _characters;
	private readonly World _world;

	public SimulationBootstrapper(World world, CharacterFactory characters)
	{
		_world = world;
		_characters = characters;
	}

	public void Initialize()
	{
		if (_world.Characters.Count == 0)
			_characters.Create(new CharacterDef());
	}
}
