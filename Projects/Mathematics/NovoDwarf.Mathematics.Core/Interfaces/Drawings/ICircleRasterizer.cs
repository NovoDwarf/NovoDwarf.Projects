using System.Numerics;

namespace Mathematics.Core.Interfaces.Drawings;

public interface ICircleRasterizer
{
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1);
}