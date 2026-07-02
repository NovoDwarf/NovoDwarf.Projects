using System.Numerics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Circles;

public class DdaCircleRasterizer : ICircleRasterizer
{
	public static DdaCircleRasterizer Default { get; } = new();
	
	private DdaCircleRasterizer() { }
	
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1)
	{
		var dx = x1 - x0;
		var dy = y1 - y0;
		var r = Math.Sqrt(dx * dx + dy * dy);

		var steps = (int)Math.Ceiling(2 * Math.PI * r);

		var dt = 2 * Math.PI / steps;

		for (var i = 0; i <= steps; i++)
		{
			var t = i * dt;

			var x = x0 + r * Math.Cos(t);
			var y = y0 + r * Math.Sin(t);

			yield return new Vector3((float)Math.Round(x), (float)Math.Round(y), 1);
		}
	}
}