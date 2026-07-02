using System.Numerics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Lines;

public class BresenhamLineRasterizer : ILineRasterizer
{
	public static BresenhamLineRasterizer Default { get; } = new();

	private BresenhamLineRasterizer() { }
	
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1)
	{
		var dx = Math.Abs(x1 - x0);
		var dy = Math.Abs(y1 - y0);
		var sx = x0 < x1 ? 1 : -1;
		var sy = y0 < y1 ? 1 : -1;
		var err = dx - dy;

		int x = x0, y = y0;

		while (true)
		{
			yield return new Vector3(x, y, 1);

			if (x == x1 && y == y1)
				break;

			var e2 = 2 * err;

			if (e2 > -dy)
			{
				err -= dy;
				x += sx;
			}

			if (e2 >= dx) 
				continue;
			
			err += dx;
			y += sy;
		}
	}
}