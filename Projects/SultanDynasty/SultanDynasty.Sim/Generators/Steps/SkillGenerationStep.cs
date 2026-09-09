using SultanDynasty.Instances.Common;
using SultanDynasty.Simulation.Generators.Interfaces;
using SultanDynasty.Simulation.Services;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class SkillGenerationStep : ICharacterGenerationStep
{
	private readonly DefaultCharacterDefinitionCatalog _defs;

	public SkillGenerationStep(DefaultCharacterDefinitionCatalog defs)
	{
		_defs = defs;
	}

	public int Order => 80;

	public void Execute(CharacterBuilderContext context)
	{
		context.Profile.Skills =
		[
			.. _defs.Skills.Select(def => new Skill
			{
				Def = def,
				Level = def.DefaultLevel + context.Random.NextFloat(0f, 2f)
			})
		];
	}
}
