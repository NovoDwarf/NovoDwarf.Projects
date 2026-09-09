using Grekov.Core;

namespace VirtualEconomic.Definitions;

public sealed class OccupationDef : Def
{
	public int SkillLevel { get; init; }

	public decimal BaseWage { get; init; }
}