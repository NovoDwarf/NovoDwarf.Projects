using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class NameGenerationStep : ICharacterGenerationStep
{
	public int Order => 20;

	public void Execute(CharacterBuilderContext context)
	{
		var identity = context.Profile.Identity;
		
		var names = identity.Gender == Enums.Gender.Female
			? identity.Ethnicity.FemaleNames
			: identity.Ethnicity.MaleNames;
		
		if (string.IsNullOrWhiteSpace(identity.FirstName) && names.Count > 0)
			identity.FirstName = context.Random.Pick(names);

		if (string.IsNullOrWhiteSpace(identity.LastName))
			identity.LastName = !string.IsNullOrWhiteSpace(identity.Dynasty.FamilyName)
				? identity.Dynasty.FamilyName
				: context.Random.Pick(identity.Ethnicity.FamilyNames);
	}
}
