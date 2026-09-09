using Grekov.Core;

namespace VirtualEconomic.Definitions;

public class GoodDef : Def
{
	public GoodCategoryDef Category { get; init; }

	public float Weight { get; init; }
	public float Volume { get; init; }

	public decimal BasePrice { get; init; }
}