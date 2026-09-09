using SultanDynasty.Enums;

namespace SultanDynasty.Instances.Characters;

public class Preferences
{
	public List<Preference<FoodPreference>> Food { get; set; } = [];
	public List<Preference<MusicPreference>> Music { get; set; } = [];
	public List<Preference<SocialPreference>> Social { get; set; } = [];

	public float PreferredAgeMaturity { get; set; }
	public float PreferredHeight { get; set; }
	public float PreferredBodyFat { get; set; }
	public float PreferredMuscularity { get; set; }

	public float PrefersDominance { get; set; }
	public float PrefersGentleness { get; set; }
	public float PrefersStatus { get; set; }
	public float PrefersIntellect { get; set; }
	public float PrefersBeauty { get; set; }
	public float PrefersFamiliarity { get; set; }
}

public class Preference<TPreference> where TPreference : struct, Enum
{
	public TPreference Type { get; set; }
	public PreferenceStrength Strength { get; set; }
	public float Weight { get; set; }
}
