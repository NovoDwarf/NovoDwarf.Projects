using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Komissar;
using SultanDynasty.Definitons;
using SultanDynasty.Instances;
using SultanDynasty.Instances.Characters;
using SultanDynasty.Simulation;
using SultanDynasty.Simulation.Services;

namespace SultanDynasty.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	private const int VisibleRoomTokenLimit = 5;

	private readonly CharacterFactory _characterFactory;
	private readonly CharacterRegistry _characters;
	private readonly Clock _clock;
	private readonly AvaloniaSimulationHost _host;
	private readonly Dictionary<int, string> _characterRooms = [];
	private readonly string[] _residentRoomCycle = ["garden", "concubines", "hammam", "kitchen", "study", "guards"];
	private string _sultanRoomId = "hall";
	private int _nextResidentRoomIndex;

	public MainWindowViewModel(
		Clock clock,
		AvaloniaSimulationHost host,
		CharacterRegistry characters,
		CharacterFactory characterFactory)
	{
		_clock = clock;
		_host = host;
		_characters = characters;
		_characterFactory = characterFactory;

		Rooms =
		[
			new("hall", "Reception Hall", "Public", 0, 0, "Entry point, servants and guards.", MoveSultanCommand),
			new("garden", "Inner Garden", "Open", 0, 1, "Good visibility, quiet movement.", MoveSultanCommand),
			new("hammam", "Hammam", "Private", 0, 2, "Restricted, voices carry through stone.", MoveSultanCommand),
			new("concubines", "Concubine Quarters", "Living", 1, 0, "Dense private wing.", MoveSultanCommand),
			new("study", "Private Study", "Royal", 1, 1, "Sultan's working room.", MoveSultanCommand),
			new("kitchen", "Kitchen", "Service", 1, 2, "Busy service room.", MoveSultanCommand),
			new("guards", "Guard Post", "Security", 2, 0, "Controlled passage.", MoveSultanCommand),
			new("nursery", "Nursery", "Family", 2, 1, "Quiet family space.", MoveSultanCommand),
			new("storage", "Storage", "Service", 2, 2, "Low visibility back room.", MoveSultanCommand)
		];

		_host.Updated += (_, _) => RefreshSimulation();

		RefreshSimulation();
	}

	public string CurrentTime { get; private set; } = string.Empty;
	public string Status { get; private set; } = string.Empty;
	public int CharacterCount { get; private set; }
	public string CurrentLevel { get; private set; } = "Building / Harem";
	public string SultanLocation { get; private set; } = string.Empty;
	public string VisionSummary { get; private set; } = string.Empty;

	public string CharacterName { get; private set; } = "No character selected";
	public string CharacterSubtitle { get; private set; } = "Select a visible token or generate a resident.";
	public string CultureLine { get; private set; } = string.Empty;
	public string BodyLine { get; private set; } = string.Empty;
	public string StateLine { get; private set; } = string.Empty;

	public ObservableCollection<RoomViewModel> Rooms { get; }
	public ObservableCollection<DetailItemViewModel> IdentityDetails { get; } = [];
	public ObservableCollection<DetailItemViewModel> BodyDetails { get; } = [];
	public ObservableCollection<DetailItemViewModel> AppearanceDetails { get; } = [];
	public ObservableCollection<MetricItemViewModel> PsychologyMetrics { get; } = [];
	public ObservableCollection<MetricItemViewModel> StateMetrics { get; } = [];
	public ObservableCollection<TagItemViewModel> Traits { get; } = [];
	public ObservableCollection<TagItemViewModel> Skills { get; } = [];
	public ObservableCollection<TagItemViewModel> Quirks { get; } = [];
	public ObservableCollection<PreferenceItemViewModel> FoodPreferences { get; } = [];
	public ObservableCollection<PreferenceItemViewModel> MusicPreferences { get; } = [];
	public ObservableCollection<PreferenceItemViewModel> SocialPreferences { get; } = [];

	[RelayCommand]
	private void Start()
	{
		_host.Start();
		RefreshSimulation();
	}

	[RelayCommand]
	private void Stop()
	{
		_host.Stop();
		RefreshSimulation();
	}

	[RelayCommand]
	private void Step()
	{
		_host.Step(TimeSpan.FromMinutes(10));
		RefreshSimulation();
	}

	[RelayCommand]
	private void GenerateCharacter()
	{
		var record = _characterFactory.Create(new CharacterDef());
		AssignRoom(record);
		RefreshCharacter(record);
		RefreshSimulation();
	}

	[RelayCommand]
	private void MoveSultan(RoomViewModel room)
	{
		_sultanRoomId = room.Id;
		RefreshSimulation();
	}

	[RelayCommand]
	private void SelectToken(CharacterTokenViewModel token)
	{
		RefreshCharacter(token.Record);
	}

	private void RefreshSimulation()
	{
		AssignMissingRooms();
		RefreshRooms();

		CurrentTime = _clock.Elapsed.ToString("d\\.hh\\:mm\\:ss", CultureInfo.InvariantCulture);
		Status = _host.IsRunning ? "Running" : "Stopped";
		CharacterCount = _characters.Count;
		SultanLocation = Rooms.First(static room => room.IsSultanHere).Name;

		var visibleCharacters = Rooms.Sum(static room => room.VisibleCharacterCount);
		var heardRooms = Rooms.Count(static room => room.Visibility == RoomVisibility.Heard);
		VisionSummary = $"{visibleCharacters} visible / {heardRooms} heard rooms";

		if (IdentityDetails.Count == 0 && _characters.Records.Count > 0)
			RefreshCharacter(_characters.Records[^1]);

		OnPropertyChanged(nameof(CurrentTime));
		OnPropertyChanged(nameof(Status));
		OnPropertyChanged(nameof(CharacterCount));
		OnPropertyChanged(nameof(CurrentLevel));
		OnPropertyChanged(nameof(SultanLocation));
		OnPropertyChanged(nameof(VisionSummary));
	}

	private void RefreshRooms()
	{
		foreach (var room in Rooms)
		{
			var visibility = GetVisibility(room);
			var records = _characters.Records
				.Where(record => _characterRooms.TryGetValue(record.Entity.Id, out var roomId) && roomId == room.Id)
				.ToArray();

			room.Refresh(
				visibility,
				room.Id == _sultanRoomId,
				records.Select(CreateToken),
				VisibleRoomTokenLimit);
		}
	}

	private void AssignMissingRooms()
	{
		foreach (var record in _characters.Records)
			AssignRoom(record);
	}

	private void AssignRoom(CharacterRecord record)
	{
		if (_characterRooms.ContainsKey(record.Entity.Id))
			return;

		var roomId = _residentRoomCycle[_nextResidentRoomIndex % _residentRoomCycle.Length];
		_nextResidentRoomIndex++;
		_characterRooms.Add(record.Entity.Id, roomId);
	}

	private CharacterTokenViewModel CreateToken(CharacterRecord record)
	{
		var identity = record.Character.Profile.Identity;
		var name = string.IsNullOrWhiteSpace(identity.FullName) ? $"E{record.Entity.Id}" : identity.FullName;
		var initials = string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(static part => part[0]));

		if (string.IsNullOrWhiteSpace(initials))
			initials = "?";

		return new CharacterTokenViewModel(record, initials.ToUpperInvariant(), name, SelectTokenCommand);
	}

	private RoomVisibility GetVisibility(RoomViewModel room)
	{
		if (room.Id == _sultanRoomId)
			return RoomVisibility.Visible;

		var current = Rooms.First(x => x.Id == _sultanRoomId);
		var distance = Math.Abs(room.Row - current.Row) + Math.Abs(room.Column - current.Column);

		return distance == 1 ? RoomVisibility.Heard : RoomVisibility.Unknown;
	}

	private void RefreshCharacter(CharacterRecord record)
	{
		var character = record.Character;
		var identity = character.Profile.Identity;
		var genetics = character.Profile.Genetics;
		var physiology = character.Profile.Physiology;
		var appearance = character.Profile.Appearance;
		var psychology = character.Profile.Psychology;
		var preferences = character.Profile.Preferences;
		var needs = character.State.Needs;
		var emotions = character.State.Emotions;

		CharacterName = string.IsNullOrWhiteSpace(identity.FullName) ? $"Entity {record.Entity.Id}" : identity.FullName;
		CharacterSubtitle = $"{identity.Gender} / {Age(identity.BirthDate)} years / {identity.CurrentTitle.Name}";
		CultureLine = $"{identity.Ethnicity.Name}, {identity.Religion.Name}, {identity.Dynasty.Name}";
		BodyLine = $"{appearance.CurrentHeight:0.#} cm, {appearance.CurrentWeight:0.#} kg, {appearance.BodyFrame}, {appearance.BodyType}";
		StateLine = $"Mood {Percent(emotions.Mood)}, Energy {Percent(needs.Energy)}, Stress {Percent(needs.Stress)}";

		Replace(IdentityDetails,
		[
			new("Gender", identity.Gender.ToString()),
			new("Birth date", FormatDate(identity.BirthDate)),
			new("Birth place", identity.BirthLocation.Name),
			new("Ethnicity", identity.Ethnicity.Name),
			new("Religion", identity.Religion.Name),
			new("Dynasty", identity.Dynasty.Name),
			new("Languages", string.Join(", ", identity.KnownLanguages.Select(static language => language.Name)))
		]);

		Replace(BodyDetails,
		[
			new("Height potential", Cm(genetics.HeightPotential)),
			new("Body frame", genetics.BodyFrame.ToString()),
			new("Body type", genetics.BodyType.ToString()),
			new("Immunity", Percent(genetics.Immunity)),
			new("Fertility", Percent(genetics.Fertility)),
			new("Libido", Percent(genetics.Libido)),
			new("Temperament", Percent(genetics.Temperament)),
			new("Aging rate", genetics.AgingRate.ToString("0.##", CultureInfo.InvariantCulture)),
			new("Temperature", $"{physiology.Temperature:0.#} C"),
			new("Pulse", $"{physiology.Pulse:0} bpm")
		]);

		Replace(AppearanceDetails,
		[
			new("Height", Cm(appearance.CurrentHeight)),
			new("Weight", Kg(appearance.CurrentWeight)),
			new("Fat", $"{appearance.FatPercent:0.#}%"),
			new("Muscle", Percent(appearance.MuscleMass)),
			new("Chest / waist / hip", $"{appearance.ChestCircumference:0.#} / {appearance.WaistCircumference:0.#} / {appearance.HipCircumference:0.#} cm"),
			new("Hair", $"{appearance.HairColor}, {appearance.HairType}, {appearance.HairLength:0.#} cm"),
			new("Eyes", appearance.EyeColor.ToString()),
			new("Skin", appearance.SkinTone.ToString()),
			new("Face", $"{appearance.FaceShape}, {appearance.NoseShape}, {appearance.LipShape}"),
			new("Voice", $"{appearance.VoiceType}, softness {Percent(appearance.VoiceSoftness)}"),
			new("Grooming", Percent(appearance.Grooming)),
			new("Cleanliness", Percent(appearance.Cleanliness))
		]);

		Replace(PsychologyMetrics,
		[
			Metric("Optimism", psychology.Optimism),
			Metric("Extraversion", psychology.Extraversion),
			Metric("Agreeableness", psychology.Agreeableness),
			Metric("Conscientiousness", psychology.Conscientiousness),
			Metric("Stability", psychology.EmotionalStability),
			Metric("Openness", psychology.Openness),
			Metric("Ambition", psychology.Ambition),
			Metric("Discipline", psychology.Discipline),
			Metric("Patience", psychology.Patience),
			Metric("Curiosity", psychology.Curiosity),
			Metric("Empathy", psychology.Empathy),
			Metric("Pride", psychology.Pride),
			Metric("Dominance", psychology.Dominance),
			Metric("Independence", psychology.Independence),
			Metric("Trust", psychology.Trust),
			Metric("Jealousy", psychology.Jealousy),
			Metric("Loyalty", psychology.Loyalty),
			Metric("Courage", psychology.Courage),
			Metric("Sensuality", psychology.Sensuality),
			Metric("Romance", psychology.Romance)
		]);

		Replace(StateMetrics,
		[
			Metric("Hunger", needs.Hunger),
			Metric("Hydration", needs.Hydration),
			Metric("Fatigue", needs.Fatigue),
			Metric("Energy", needs.Energy),
			Metric("Sleepiness", needs.Sleepiness),
			Metric("Stress", needs.Stress),
			Metric("Arousal", needs.Arousal),
			Metric("Pain", needs.Pain),
			Metric("Mood", emotions.Mood),
			Metric("Joy", emotions.Joy),
			Metric("Anger", emotions.Anger),
			Metric("Fear", emotions.Fear),
			Metric("Affection", emotions.Affection),
			Metric("Shame", emotions.Shame)
		]);

		Replace(Traits, character.Profile.Traits.Select(static trait => new TagItemViewModel(trait.Def.Name, Percent(trait.Intensity))));
		Replace(Skills, character.Profile.Skills.Select(static skill => new TagItemViewModel(skill.Def.Name, $"Level {skill.Level:0.#}")));
		Replace(Quirks, character.Profile.Quirks.Select(static quirk => new TagItemViewModel(quirk.Def.Name, Percent(quirk.Intensity))));
		Replace(FoodPreferences, preferences.Food.Select(static preference => Preference(preference.Type.ToString(), preference.Strength.ToString(), preference.Weight)));
		Replace(MusicPreferences, preferences.Music.Select(static preference => Preference(preference.Type.ToString(), preference.Strength.ToString(), preference.Weight)));
		Replace(SocialPreferences, preferences.Social.Select(static preference => Preference(preference.Type.ToString(), preference.Strength.ToString(), preference.Weight)));

		OnPropertyChanged(nameof(CharacterName));
		OnPropertyChanged(nameof(CharacterSubtitle));
		OnPropertyChanged(nameof(CultureLine));
		OnPropertyChanged(nameof(BodyLine));
		OnPropertyChanged(nameof(StateLine));
	}

	private static int Age(DateOnly birthDate)
	{
		if (birthDate == default)
			return 0;

		var today = DateOnly.FromDateTime(DateTime.Today);
		var age = today.Year - birthDate.Year;
		return birthDate > today.AddYears(-age) ? age - 1 : age;
	}

	private static MetricItemViewModel Metric(string name, float value)
	{
		var normalized = Math.Clamp(value, 0f, 1f);
		return new MetricItemViewModel(name, normalized * 100f, Percent(normalized));
	}

	private static PreferenceItemViewModel Preference(string name, string strength, float weight)
	{
		var normalized = Math.Clamp(weight, 0f, 1f);
		return new PreferenceItemViewModel(name, strength, normalized * 100f, Percent(normalized));
	}

	private static string Cm(float value) => $"{value:0.#} cm";
	private static string Kg(float value) => $"{value:0.#} kg";
	private static string FormatDate(DateOnly value) => value == default ? string.Empty : value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
	private static string Percent(float value) => $"{Math.Clamp(value, 0f, 1f) * 100f:0}%";

	private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> values)
	{
		target.Clear();

		foreach (var value in values)
			target.Add(value);
	}
}

public enum RoomVisibility
{
	Visible,
	Heard,
	Unknown
}

public sealed class RoomViewModel : ViewModelBase
{
	public RoomViewModel(string id, string name, string zone, int row, int column, string description, ICommand moveCommand)
	{
		Id = id;
		Name = name;
		Zone = zone;
		Row = row;
		Column = column;
		Description = description;
		MoveCommand = moveCommand;
	}

	public string Id { get; }
	public string Name { get; }
	public string Zone { get; }
	public int Row { get; }
	public int Column { get; }
	public string Description { get; }
	public ICommand MoveCommand { get; }
	public bool IsSultanHere { get; private set; }
	public RoomVisibility Visibility { get; private set; } = RoomVisibility.Unknown;
	public string VisibilityText { get; private set; } = "Unknown";
	public string SultanMarker { get; private set; } = string.Empty;
	public string PopulationText { get; private set; } = "No exact data";
	public string OverflowText { get; private set; } = string.Empty;
	public int VisibleCharacterCount { get; private set; }
	public ObservableCollection<CharacterTokenViewModel> Tokens { get; } = [];

	public void Refresh(RoomVisibility visibility, bool isSultanHere, IEnumerable<CharacterTokenViewModel> tokens, int tokenLimit)
	{
		var allTokens = tokens.ToArray();
		var visibleTokens = visibility == RoomVisibility.Visible
			? allTokens.Take(tokenLimit).ToArray()
			: [];

		Visibility = visibility;
		IsSultanHere = isSultanHere;
		VisibilityText = visibility switch
		{
			RoomVisibility.Visible => "Visible",
			RoomVisibility.Heard => "Heard",
			_ => "Unknown"
		};
		SultanMarker = isSultanHere ? "Sultan here" : string.Empty;
		VisibleCharacterCount = visibility == RoomVisibility.Visible ? allTokens.Length : 0;
		PopulationText = visibility switch
		{
			RoomVisibility.Visible => allTokens.Length == 0 ? "Empty" : $"{allTokens.Length} residents",
			RoomVisibility.Heard => "Noise and movement",
			_ => "No exact data"
		};
		OverflowText = visibility == RoomVisibility.Visible && allTokens.Length > tokenLimit
			? $"+{allTokens.Length - tokenLimit}"
			: string.Empty;

		ReplaceTokens(visibleTokens);

		OnPropertyChanged(nameof(Visibility));
		OnPropertyChanged(nameof(IsSultanHere));
		OnPropertyChanged(nameof(VisibilityText));
		OnPropertyChanged(nameof(SultanMarker));
		OnPropertyChanged(nameof(PopulationText));
		OnPropertyChanged(nameof(OverflowText));
		OnPropertyChanged(nameof(VisibleCharacterCount));
	}

	private void ReplaceTokens(IEnumerable<CharacterTokenViewModel> tokens)
	{
		Tokens.Clear();

		foreach (var token in tokens)
			Tokens.Add(token);
	}
}

public sealed class CharacterTokenViewModel
{
	public CharacterTokenViewModel(CharacterRecord record, string initials, string tooltip, ICommand selectCommand)
	{
		Record = record;
		Initials = initials;
		Tooltip = tooltip;
		SelectCommand = selectCommand;
	}

	public CharacterRecord Record { get; }
	public string Initials { get; }
	public string Tooltip { get; }
	public ICommand SelectCommand { get; }
}

public sealed class DetailItemViewModel
{
	public DetailItemViewModel(string name, string value)
	{
		Name = name;
		Value = value;
	}

	public string Name { get; }
	public string Value { get; }
}

public sealed class MetricItemViewModel
{
	public MetricItemViewModel(string name, float value, string valueText)
	{
		Name = name;
		Value = value;
		ValueText = valueText;
	}

	public string Name { get; }
	public float Value { get; }
	public string ValueText { get; }
}

public sealed class TagItemViewModel
{
	public TagItemViewModel(string name, string value)
	{
		Name = name;
		Value = value;
	}

	public string Name { get; }
	public string Value { get; }
}

public sealed class PreferenceItemViewModel
{
	public PreferenceItemViewModel(string name, string strength, float weight, string weightText)
	{
		Name = name;
		Strength = strength;
		Weight = weight;
		WeightText = weightText;
	}

	public string Name { get; }
	public string Strength { get; }
	public float Weight { get; }
	public string WeightText { get; }
}
