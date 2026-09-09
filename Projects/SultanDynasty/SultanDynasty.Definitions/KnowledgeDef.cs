using Grekov.Core;

namespace SultanDynasty.Definitons;

public class KnowledgeDef : Def
{
	public bool IsSecret { get; init; }
	public bool CanSpread { get; init; }

	public float BaseConfidence { get; init; }
}