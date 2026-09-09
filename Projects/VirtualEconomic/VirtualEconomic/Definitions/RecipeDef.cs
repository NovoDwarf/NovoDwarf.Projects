using Grekov.Core;

namespace VirtualEconomic.Definitions;

public sealed class RecipeDef : Def
{
	public IReadOnlyList<RecipeDef> Inputs { get; init; }
	public IReadOnlyList<RecipeDef> Outputs { get; init; }

	public TimeSpan Duration { get; init; }

	public int WorkforceRequired { get; init; }
}