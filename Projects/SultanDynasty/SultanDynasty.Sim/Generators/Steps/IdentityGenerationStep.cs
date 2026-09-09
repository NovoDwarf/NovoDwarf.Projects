using SultanDynasty.Enums;
using SultanDynasty.Simulation.Generators.Interfaces;
using SultanDynasty.Simulation.Services;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class IdentityGenerationStep : ICharacterGenerationStep
{
	private readonly DefaultCharacterDefinitionCatalog _defs;

	public IdentityGenerationStep(DefaultCharacterDefinitionCatalog defs)
	{
		_defs = defs;
	}

	public int Order => 10;

	public void Execute(CharacterBuilderContext context)
	{
		var identity = context.Profile.Identity;

		identity.Gender = context.Random.PickEnum<Gender>();
		identity.Ethnicity = context.Random.Pick(_defs.Ethnicities);
		identity.Religion = context.Random.Pick(_defs.Religions);
		identity.Dynasty = context.Random.Pick(_defs.Dynasties);
		identity.BirthLocation = context.Random.Pick(_defs.Locations);
		identity.KnownLanguages = [context.Random.Pick(_defs.Languages)];

		var age = context.Random.Next(16, 36);
		identity.BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-age).AddDays(-context.Random.Next(365)));
	}
}
