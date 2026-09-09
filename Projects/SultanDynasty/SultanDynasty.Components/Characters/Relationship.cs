namespace SultanDynasty.Instances.Characters;

public class Relationship
{
	public Guid Target { get; init; }
	
	public float Trust { get; private set; }
	public float Respect { get; private set; }
	public float Fear { get; private set; }
	public float Love { get; private set; }
	public float Envy { get; private set; }
	public float Hatred { get; private set; }
}