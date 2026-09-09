using Grekov.Core;

namespace SultanDynasty.Definitons;

public class MemoryDef : Def
{
	public float DefaultImportance { get; init; }

	public float DefaultEmotionalImpact { get; init; }

	public bool IsPositive { get; init; }

	public bool CanDecay { get; init; }

	public TimeSpan? Lifetime { get; init; }
}