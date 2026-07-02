namespace Mathematics.Graphics.Neighborhoods;

using System.Numerics;

public class NeumannNeighborhood : INeighborhood2D, INeighborhood3D
{
	public static NeumannNeighborhood Default { get; } = new();
	
	private static readonly Vector2[] OffsetsInternal2D = 	
	[
		new( 0, -1),
		new(-1,  0),            new( 1,  0),
		new( 0,  1)
	];

	private static readonly Vector3[] OffsetsInternal3D =
	[
		new( 0,  0, -1),
		new( 0,  0,  1),
		new( 0, -1,  0),
		new( 0,  1,  0),
		new(-1,  0,  0),
		new( 1,  0,  0)
	];
	
	private NeumannNeighborhood() { }
	
	public Vector2[] Offsets2D => OffsetsInternal2D;

	public Vector3[] Offsets3D => OffsetsInternal3D;
}