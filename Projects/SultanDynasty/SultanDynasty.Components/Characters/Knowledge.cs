using SultanDynasty.Definitons;

namespace SultanDynasty.Instances.Characters;

public class Knowledge
{
	public KnowledgeDef Def { get; init; }

	public float Confidence { get; set; }
	
	public Guid? SubjectId { get; init; }
}