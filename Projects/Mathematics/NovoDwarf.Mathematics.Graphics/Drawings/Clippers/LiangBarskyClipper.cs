using Mathematics.Core.Base.Graphics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Clippers;

public class LiangBarskyClipper : ILineClipper
{
	public Rect Rect { get; set; }
	
	public bool Clip(ref float x0, ref float y0, ref float x1, ref float y1)
	{
		var dx = x1 - x0;
		var dy = y1 - y0;
		float u1 = 0, u2 = 1;

		float[] p = [-dx, dx, -dy, dy];
		float[] q = [x0 - Rect.MinX, Rect.MaxX - x0, y0 - Rect.MinY, Rect.MaxY - y0];

		for (var i = 0; i < 4; i++)
		{
			if (p[i] == 0)
			{
				if (q[i] < 0) 
					return false; 
			}
			else
			{
				var u = q[i] / p[i];
				
				if (p[i] < 0) u1 = Math.Max(u1, u);
				else u2 = Math.Min(u2, u);
			}
		}

		if (u1 > u2) 
			return false;

		x1 = x0 + u2 * dx;
		y1 = y0 + u2 * dy;
		
		x0 += u1 * dx;
		y0 += u1 * dy;

		return true;
	}
}