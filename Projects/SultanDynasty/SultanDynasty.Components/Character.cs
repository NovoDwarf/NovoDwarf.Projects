using Friflo.Engine.ECS;
using SultanDynasty.Instances.Characters;
using SultanDynasty.Instances.Common;

namespace SultanDynasty.Instances;

public sealed class CharacterRecord
{
	public Entity Entity { get; }
	public Character Character { get; }

	public bool IsAlive { get; set; } = true;
	public bool IsLoaded { get; set; } = true;

	public int Version { get; internal set; }

	public CharacterRecord(Entity entity, Character character)
	{
		Entity = entity;
		Character = character;
	}
}

public class Character
{
	public CharacterProfile Profile { get; set; } = new();
	public CharacterMind Mind { get; set; } = new();
	public CharacterState State { get; set; } = new();
}

public class CharacterProfile
{
	public Identity Identity { get; set; } = new();
	public Appearance Appearance { get; set; } = new();
	
	public Psychology Psychology { get; set; } = new();
	public Physiology Physiology { get; set; } = new();
	public Genetics Genetics { get; set; } = new();
	public Preferences Preferences { get; set; } = new();
	
	public List<Skill> Skills { get; set; } = [];
	public List<Trait> Traits { get; set; } = [];
	public List<Quirk> Quirks { get; set; } = [];
}

public sealed class CharacterMind
{
	public Relationships Relationships { get; init; } = new();
	public Memories Memories { get; init; } = new();
	public Knowledges Knowledges { get; init; } = new();
	public Goals Goals { get; init; } = new();
}

public sealed class CharacterState
{
	public Needs Needs { get; set; }
	public Emotions Emotions { get; set; }
}
