using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class PhysiologyGenerationStep : ICharacterGenerationStep
{
	public int Order => 35;

	public void Execute(CharacterBuilderContext context)
	{
		var genetics = context.Profile.Genetics;

		context.Profile.Physiology = new()
		{
			Immunity = genetics.Immunity,
			Temperature = 36.6f,
			Pulse = 70f
		};
	}
}
