using System.Numerics;

namespace Mathematics.Graphics.Neighborhoods;

public interface INeighborhood2D
{
	public Vector2[] Offsets2D { get; }
}