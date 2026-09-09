using Grekov.Core;

namespace VirtualEconomic.Definitions;

public sealed class TechnologyDef : Def
{
	public string Name { get; init; }

	public IReadOnlyList<TechnologyDef> Prerequisites { get; init; }

	public TimeSpan ResearchTime { get; init; }
}