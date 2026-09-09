using SultanDynasty.Enums;
using SultanDynasty.Instances.Characters;
using SultanDynasty.Simulation.Generators.Interfaces;

namespace SultanDynasty.Simulation.Generators.Steps;

public sealed class PreferencesGenerationStep : ICharacterGenerationStep
{
	public int Order => 60;

	public void Execute(CharacterBuilderContext context)
	{
		context.Profile.Preferences = new()
		{
			Food = CreatePreferences<FoodPreference>(context),
			Music = CreatePreferences<MusicPreference>(context),
			Social = CreatePreferences<SocialPreference>(context),
			PreferredAgeMaturity = context.Random.NextFloat(0.35f, 0.8f),
			PreferredHeight = context.Random.NextFloat(155f, 190f),
			PreferredBodyFat = context.Random.NextFloat(0.15f, 0.35f),
			PreferredMuscularity = context.Random.NextFloat(0.2f, 0.8f),
			PrefersDominance = context.Random.NextFloat(0f, 1f),
			PrefersGentleness = context.Random.NextFloat(0f, 1f),
			PrefersStatus = context.Random.NextFloat(0f, 1f),
			PrefersIntellect = context.Random.NextFloat(0f, 1f),
			PrefersBeauty = context.Random.NextFloat(0f, 1f),
			PrefersFamiliarity = context.Random.NextFloat(0f, 1f)
		};
	}

	private static List<Preference<T>> CreatePreferences<T>(CharacterBuilderContext context) where T : struct, Enum
	{
		return Enum.GetValues<T>()
			.OrderBy(_ => context.Random.Next())
			.Take(3)
			.Select(value => new Preference<T>
			{
				Type = value,
				Strength = context.Random.PickEnum<PreferenceStrength>(),
				Weight = context.Random.NextFloat(0.2f, 1f)
			})
			.ToList();
	}
}
