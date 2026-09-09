using Friflo.Engine.ECS;
using SultanDynasty.Definitons;
using SultanDynasty.Instances;
using SultanDynasty.Instances.Characters;

namespace SultanDynasty.Simulation.Generators;

public sealed class CharacterBuilderContext
{
	public required Random Random { get; init; }
	public required World World { get; init; }
	public required Entity Entity { get; init; }
	public required CharacterDef BaseDef { get; init; }
	public required Character Character { get; init; }
	
	public CharacterProfile Profile => Character.Profile;
	public Needs? Needs { get; set; }
	public Emotions? Emotions { get; set; }

	public required IServiceProvider Services { get; init; }

	public Dictionary<object, object> Data { get; } = [];
}
