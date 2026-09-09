using SultanDynasty.Instances.Characters;
using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class NeedsGenerationStep : ICharacterGenerationStep
{
	public int Order => 100;

	public void Execute(CharacterBuilderContext context)
	{
		context.Needs = new Needs
		{
			Hunger = context.Random.NextFloat(0.1f, 0.35f),
			Hydration = context.Random.NextFloat(0.1f, 0.35f),
			Fatigue = context.Random.NextFloat(0.1f, 0.45f),
			Energy = context.Random.NextFloat(0.55f, 1f),
			Sleepiness = context.Random.NextFloat(0.1f, 0.4f),
			Stress = context.Random.NextFloat(0.05f, 0.35f),
			Arousal = context.Random.NextFloat(0f, 0.25f),
			Pain = 0f
		};
	}
}
