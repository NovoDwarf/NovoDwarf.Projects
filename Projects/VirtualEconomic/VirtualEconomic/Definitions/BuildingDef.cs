using Grekov.Core;

namespace VirtualEconomic.Definitions;

public sealed class BuildingDef : Def
{
	public int Width { get; init; }
	public int Height { get; init; }

	public IReadOnlyList<RecipeDef> Recipes { get; init; }

	public BuildingCategoryDef Category { get; init; }
}