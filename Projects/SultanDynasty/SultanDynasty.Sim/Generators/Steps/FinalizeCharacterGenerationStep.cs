using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class FinalizeCharacterGenerationStep : ICharacterGenerationStep
{
	public int Order => 1000;

	public void Execute(CharacterBuilderContext context)
	{
		context.Entity.AddComponent(context.Profile.Genetics);
		context.Entity.AddComponent(context.Profile.Physiology);
		context.Entity.AddComponent(context.Profile.Appearance);
		context.Entity.AddComponent(context.Profile.Psychology);

		if (context.Needs.HasValue)
		{
			var needs = context.Needs.Value;
			context.Character.State.Needs = needs;
			context.Entity.AddComponent(needs);
		}

		if (context.Emotions.HasValue)
		{
			var emotions = context.Emotions.Value;
			context.Character.State.Emotions = emotions;
			context.Entity.AddComponent(emotions);
		}
	}
}
