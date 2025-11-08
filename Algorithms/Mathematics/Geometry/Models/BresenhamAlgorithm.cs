using System.Drawing;

namespace Mathematics.Geometry.Models;

public class BresenhamAlgorithm
{
	public static List<Point> DrawLine(int x0, int y0, int x1, int y1)
	{
		var points = new List<Point>();
		var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

		if (steep)
		{
			(x0, y0) = (y0, x0);
			(x1, y1) = (y1, x1);
		}

		if (x0 > x1)
		{
			(x0, y1) = (y1, x0);
			(y0, y1) = (y1, y0);
		}

		var dx = x1 - x0;
		var dy = Math.Abs(y1 - y0);
		var error = dx / 2;
		var ystep = (y0 < y1) ? 1 : -1;
		var y = y0;

		for (var x = x0; x <= x1; x++)
		{
			var point = steep ? new Point(y, x) : new Point(x, y);
			points.Add(point);

			error -= dy;

			if (error >= 0)
				continue;

			y += ystep;
			error += dx;
		}

		return points;
	}
}