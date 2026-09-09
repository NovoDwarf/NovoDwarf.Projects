using SultanDynasty.Instances.Common;
using SultanDynasty.Simulation.Generators.Interfaces;
using SultanDynasty.Simulation.Services;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class QuirkGenerationStep : ICharacterGenerationStep
{
	private readonly DefaultCharacterDefinitionCatalog _defs;

	public QuirkGenerationStep(DefaultCharacterDefinitionCatalog defs)
	{
		_defs = defs;
	}

	public int Order => 90;

	public void Execute(CharacterBuilderContext context)
	{
		context.Profile.Quirks =
		[
			.. _defs.Quirks
				.OrderBy(_ => context.Random.Next())
				.Take(1)
				.Select(def => new Quirk
				{
					Def = def,
					Intensity = def.DefaultIntensity + context.Random.NextFloat(-0.1f, 0.1f)
				})
		];
	}
}
