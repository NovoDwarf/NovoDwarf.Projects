using System.Numerics;

namespace Mathematics.Graphics.Neighborhoods;

public interface INeighborhood3D
{
	public Vector3[] Offsets3D { get; }
}