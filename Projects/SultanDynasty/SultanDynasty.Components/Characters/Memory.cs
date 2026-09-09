using SultanDynasty.Definitons;

namespace SultanDynasty.Instances.Characters;

public class Memory
{
	public MemoryDef Def { get; init; }

	public Guid? SubjectId { get; set; }
	public Guid? TargetId { get; set; }

	public DateTime CreatedAt { get; init; }

	public float Importance { get; set; }
	public float Confidence { get; set; }
}