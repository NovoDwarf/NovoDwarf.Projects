using Friflo.Engine.ECS;

namespace SultanDynasty.Instances.Characters;

public struct Physiology : IComponent
{
	public float IllnessSeverity;
	public float PregnancyProgress;
	public float CycleProgress;

	public float Alcohol;

	public float Temperature;
	public float Immunity;
	public float Pulse;
}
