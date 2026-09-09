using SultanDynasty.Instances.Common;
using SultanDynasty.Simulation.Generators.Interfaces;
using SultanDynasty.Simulation.Services;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class TraitGenerationStep : ICharacterGenerationStep
{
	private readonly DefaultCharacterDefinitionCatalog _defs;

	public TraitGenerationStep(DefaultCharacterDefinitionCatalog defs)
	{
		_defs = defs;
	}

	public int Order => 70;

	public void Execute(CharacterBuilderContext context)
	{
		context.Profile.Traits =
		[
			.. _defs.Traits
				.OrderBy(_ => context.Random.Next())
				.Take(2)
				.Select(def => new Trait
				{
					Def = def,
					Intensity = def.DefaultIntensity + context.Random.NextFloat(-0.1f, 0.1f)
				})
		];
	}
}
