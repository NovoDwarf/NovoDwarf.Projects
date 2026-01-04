using System.Numerics;

namespace Mathematics.Core.Base.Graphics;

public class Vertex
{
	public Vector2 Position;
	
	public bool IsIntersection;
	public bool IsEntry;
	
	public Vertex? Next;
	public Vertex? Prev;
	public Vertex? Corresponding;

	public Vertex(Vector2 pos)
	{
		Position = pos;
	}
}