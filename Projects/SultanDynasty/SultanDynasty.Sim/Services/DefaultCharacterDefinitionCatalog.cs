using Grekov.Core;
using Grekov.Definitions.Interfaces;
using SultanDynasty.Definitons.Characters;
using SultanDynasty.Definitons.Common;
using SultanDynasty.Definitons.Stats;

namespace SultanDynasty.Simulation.Services;

public sealed class DefaultCharacterDefinitionCatalog
{
	private readonly IDefCatalog _defs;
	private readonly IReadOnlyList<EthnicityDef> _fallbackEthnicities;
	private readonly IReadOnlyList<ReligionDef> _fallbackReligions;
	private readonly IReadOnlyList<DynastyDef> _fallbackDynasties;
	private readonly IReadOnlyList<LanguageDef> _fallbackLanguages;
	private readonly IReadOnlyList<LocationDef> _fallbackLocations;
	private readonly IReadOnlyList<TraitDef> _fallbackTraits;
	private readonly IReadOnlyList<SkillDef> _fallbackSkills;
	private readonly IReadOnlyList<QuirkDef> _fallbackQuirks;

	public DefaultCharacterDefinitionCatalog(IDefCatalog defs)
	{
		_defs = defs;

		_fallbackEthnicities =
		[
			new()
			{
				Id = DefId.Parse("ethnicity/rumelian"),
				Name = "Rumelian",
				FemaleNames = ["Aylin", "Leyla", "Meryem", "Rana"],
				MaleNames = ["Selim", "Kemal", "Orhan", "Murad"],
				FamilyNames = ["Hatun", "Sari", "Demir"]
			},
			new()
			{
				Id = DefId.Parse("ethnicity/persian"),
				Name = "Persian",
				FemaleNames = ["Shirin", "Pari", "Nargis", "Darya"],
				MaleNames = ["Farid", "Cem", "Rostam", "Arman"],
				FamilyNames = ["Nuri", "Ravan", "Azadi"]
			}
		];

		_fallbackReligions =
		[
			new() { Id = DefId.Parse("religion/sunni"), Name = "Sunni" },
			new() { Id = DefId.Parse("religion/shia"), Name = "Shia" }
		];

		_fallbackDynasties =
		[
			new() { Id = DefId.Parse("dynasty/osman"), Name = "House Osman", FamilyName = "Osman" },
			new() { Id = DefId.Parse("dynasty/none"), Name = "Common Lineage", FamilyName = string.Empty }
		];

		_fallbackLanguages =
		[
			new() { Id = DefId.Parse("language/turkish"), Name = "Turkish", Code = "tr" },
			new() { Id = DefId.Parse("language/persian"), Name = "Persian", Code = "fa" }
		];

		_fallbackLocations =
		[
			new() { Id = DefId.Parse("location/palace"), Name = "Imperial Palace", Region = "Capital" },
			new() { Id = DefId.Parse("location/rumelia"), Name = "Rumelian Province", Region = "Rumelia" }
		];

		_fallbackTraits =
		[
			new() { Id = DefId.Parse("trait/graceful"), Name = "Graceful", DefaultIntensity = 0.7f },
			new() { Id = DefId.Parse("trait/curious"), Name = "Curious", DefaultIntensity = 0.6f },
			new() { Id = DefId.Parse("trait/proud"), Name = "Proud", DefaultIntensity = 0.55f }
		];

		_fallbackSkills =
		[
			new() { Id = DefId.Parse("skill/etiquette"), Name = "Etiquette", Category = "Court", DefaultLevel = 2f },
			new() { Id = DefId.Parse("skill/music"), Name = "Music", Category = "Art", DefaultLevel = 1f },
			new() { Id = DefId.Parse("skill/poetry"), Name = "Poetry", Category = "Art", DefaultLevel = 1f }
		];

		_fallbackQuirks =
		[
			new() { Id = DefId.Parse("quirk/light_sleeper"), Name = "Light sleeper", DefaultIntensity = 0.45f },
			new() { Id = DefId.Parse("quirk/sweet_tooth"), Name = "Sweet tooth", DefaultIntensity = 0.6f }
		];
	}

	public IReadOnlyList<EthnicityDef> Ethnicities => LoadedOrFallback(_defs.All<EthnicityDef>(), _fallbackEthnicities);
	public IReadOnlyList<ReligionDef> Religions => LoadedOrFallback(_defs.All<ReligionDef>(), _fallbackReligions);
	public IReadOnlyList<DynastyDef> Dynasties => LoadedOrFallback(_defs.All<DynastyDef>(), _fallbackDynasties);
	public IReadOnlyList<LanguageDef> Languages => LoadedOrFallback(_defs.All<LanguageDef>(), _fallbackLanguages);
	public IReadOnlyList<LocationDef> Locations => LoadedOrFallback(_defs.All<LocationDef>(), _fallbackLocations);
	public IReadOnlyList<TraitDef> Traits => LoadedOrFallback(_defs.All<TraitDef>(), _fallbackTraits);
	public IReadOnlyList<SkillDef> Skills => LoadedOrFallback(_defs.All<SkillDef>(), _fallbackSkills);
	public IReadOnlyList<QuirkDef> Quirks => LoadedOrFallback(_defs.All<QuirkDef>(), _fallbackQuirks);

	private static IReadOnlyList<T> LoadedOrFallback<T>(IEnumerable<T> loaded, IReadOnlyList<T> fallback)
	{
		var values = loaded.ToArray();
		return values.Length == 0 ? fallback : values;
	}
}
