using SultanDynasty.Definitons.Stats;

namespace SultanDynasty.Instances.Common;

public class Quirk
{
	public required QuirkDef Def { get; init; }
	public float Intensity { get; set; }
}
