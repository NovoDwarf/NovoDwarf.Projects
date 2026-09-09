using SultanDynasty.Definitons;
using SultanDynasty.Definitons.Common;

namespace SultanDynasty.Instances.Characters;

public class Goal
{
	public GoalDef Def { get; private set; }

	public float Priority { get; private set; }
	public float Progress { get; private set; }

	public int Status { get; private set; }
}