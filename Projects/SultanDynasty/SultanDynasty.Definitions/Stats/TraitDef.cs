using Grekov.Core;
using Grekov.Core.Attributes;

namespace SultanDynasty.Definitons.Stats;

public class TraitDef : Def
{
	[DefField]
	public float DefaultIntensity { get; set; } = 0.5f;
}
