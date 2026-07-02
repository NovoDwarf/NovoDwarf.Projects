using System.Numerics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Lines;

public class DdaLineRasterizer : ILineRasterizer
{
	public static DdaLineRasterizer Default { get; } = new();
	
	private DdaLineRasterizer() { }
	
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1)
	{
		var dx = x1 - x0;
		var dy = y1 - y0;
		var steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

		var xInc = (double)dx / steps;
		var yInc = (double)dy / steps;

		double x = x0;
		double y = y0;

		for (var i = 0; i <= steps; i++)
		{
			yield return new Vector3((float)Math.Round(x), (float)Math.Round(y), 1);

			x += xInc;
			y += yInc;
		}
	}
}