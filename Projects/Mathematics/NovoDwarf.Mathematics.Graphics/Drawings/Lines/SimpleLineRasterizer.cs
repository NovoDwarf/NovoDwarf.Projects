using System.Numerics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Lines;

public class SimpleLineRasterizer : ILineRasterizer
{
	public static SimpleLineRasterizer Default { get; } = new();

	private SimpleLineRasterizer() { }
	
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1)
	{
		var dx = x1 - x0;
		var dy = y1 - y0;

		if (dx == 0)
			yield break;

		var k = (double)dy / dx;
		var step = dx > 0 ? 1 : -1;

		for (var x = x0; x != x1; x += step)
		{
			var y = y0 + k * (x - x0);
			yield return new Vector3(x, (float)Math.Round(y), 1);
		}

		yield return new Vector3(x1, y1, 1);
	}
}