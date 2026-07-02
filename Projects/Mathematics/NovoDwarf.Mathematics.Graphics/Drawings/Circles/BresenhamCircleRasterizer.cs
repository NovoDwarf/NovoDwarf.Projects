using System.Numerics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Circles;

public class BresenhamCircleRasterizer : ICircleRasterizer
{
	public static BresenhamCircleRasterizer Default { get; } = new();
	
	private BresenhamCircleRasterizer() { }
	
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1)
	{
		var dx = x1 - x0;
		var dy = x1 - y0;
		var r = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy));

		var x = 0;
		var y = r;
		var err = 1 - r;

		while (x <= y)
		{
			foreach (var p in Plot8(x0, y0, x, y))
				yield return p;

			if (err < 0)
			{
				err += 2 * x + 3;
			}
			else
			{
				err += 2 * (x - y) + 5;
				y--;
			}

			x++;
		}
	}
	
	private static IEnumerable<Vector3> Plot8(int cx, int cy, int x, int y)
	{
		yield return new Vector3(cx + x, cy + y, 1);
		yield return new Vector3(cx - x, cy + y, 1);
		yield return new Vector3(cx + x, cy - y, 1);
		yield return new Vector3(cx - x, cy - y, 1);
		yield return new Vector3(cx + y, cy + x, 1);
		yield return new Vector3(cx - y, cy + x, 1);
		yield return new Vector3(cx + y, cy - x, 1);
		yield return new Vector3(cx - y, cy - x, 1);
	}
}