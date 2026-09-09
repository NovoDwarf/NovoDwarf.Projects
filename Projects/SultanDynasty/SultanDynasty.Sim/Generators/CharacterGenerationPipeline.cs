using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators;

public sealed class CharacterGenerationPipeline
{
	private readonly List<ICharacterGenerationStep> _steps;

	public CharacterGenerationPipeline(IEnumerable<ICharacterGenerationStep> steps)
	{
		_steps = [.. steps.OrderBy(static step => step.Order)];
	}

	public void Execute(CharacterBuilderContext context)
	{
		foreach (var step in _steps)
		{
			step.Execute(context);
		}
	}
}
