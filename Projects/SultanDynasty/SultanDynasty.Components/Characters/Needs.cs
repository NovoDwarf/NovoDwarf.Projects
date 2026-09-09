using Friflo.Engine.ECS;

namespace SultanDynasty.Instances.Characters;

public struct Needs : IComponent
{
	public float Hunger;
	public float Hydration;
	public float Fatigue;
	public float Energy;
	public float Sleepiness;
	public float Stress;
	public float Arousal;
	public float Pain;
}