namespace NovoDwarf.Primitives.Models.Int3;

public readonly partial record struct Int3
{
	public override string ToString()
	{
		return $"({X}, {Y}, {Z})";
	}
}