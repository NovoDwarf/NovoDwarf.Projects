using SultanDynasty.Instances.Characters;
using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class EmotionGenerationStep : ICharacterGenerationStep
{
	public int Order => 110;

	public void Execute(CharacterBuilderContext context)
	{
		context.Emotions = new Emotions
		{
			Mood = context.Random.NextFloat(0.35f, 0.85f),
			Joy = context.Random.NextFloat(0.1f, 0.7f),
			Anger = context.Random.NextFloat(0f, 0.25f),
			Fear = context.Random.NextFloat(0f, 0.35f),
			Affection = context.Random.NextFloat(0.1f, 0.7f),
			Shame = context.Random.NextFloat(0f, 0.3f)
		};
	}
}
