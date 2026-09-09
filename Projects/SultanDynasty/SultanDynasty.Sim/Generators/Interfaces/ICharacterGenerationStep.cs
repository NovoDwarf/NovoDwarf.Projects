namespace SultanDynasty.Simulation.Generators.Interfaces;

public interface ICharacterGenerationStep
{
	public int Order => 0;
	
	public void Execute(CharacterBuilderContext context);
}
