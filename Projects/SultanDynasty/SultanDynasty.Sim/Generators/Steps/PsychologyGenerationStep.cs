using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class PsychologyGenerationStep : ICharacterGenerationStep
{
	public int Order => 50;

	public void Execute(CharacterBuilderContext context)
	{
		context.Profile.Psychology = new()
		{
			Optimism = Roll(context),
			Extraversion = Roll(context),
			Agreeableness = Roll(context),
			Conscientiousness = Roll(context),
			EmotionalStability = Roll(context),
			Openness = Roll(context),
			Ambition = Roll(context),
			Discipline = Roll(context),
			Patience = Roll(context),
			Curiosity = Roll(context),
			Empathy = Roll(context),
			Pride = Roll(context),
			Dominance = Roll(context),
			Independence = Roll(context),
			Trust = Roll(context),
			Jealousy = Roll(context),
			Loyalty = Roll(context),
			Courage = Roll(context),
			Sensuality = Roll(context),
			Romance = Roll(context),
			LibidoExpression = context.Profile.Genetics.Libido
		};
	}

	private static float Roll(CharacterBuilderContext context)
	{
		return context.Random.NextFloat(0.15f, 0.95f);
	}
}
