using Komissar;
using Friflo.Engine.ECS;
using Komissar.Time;
using SultanDynasty.Simulation.Services;

namespace SultanDynasty.Simulation;

public sealed class World
{
	public World(CharacterRegistry characters, RuleEngine rules, Clock clock)
	{
		Characters = characters;
		Rules = rules;
		Clock = clock;
		Entities = new EntityStore();
		Random = new Random();
	}

	public CharacterRegistry Characters { get; }

	public Clock Clock { get; }

	public EntityStore Entities { get; }

	public RuleEngine Rules { get; }

	public Random Random { get; }
}

public class RuleEngine
{
}
