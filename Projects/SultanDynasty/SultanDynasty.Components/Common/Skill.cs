using SultanDynasty.Definitons.Stats;

namespace SultanDynasty.Instances.Common;

public class Skill
{
	public required SkillDef Def { get; init; }
	public float Level { get; set; }
}
