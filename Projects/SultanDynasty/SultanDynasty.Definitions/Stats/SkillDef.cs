using Grekov.Core;
using Grekov.Core.Attributes;

namespace SultanDynasty.Definitons.Stats;

public class SkillDef : Def
{
	[DefField]
	public string Category { get; set; } = string.Empty;
	
	[DefField]
	public float DefaultLevel { get; set; } = 1f;
}
