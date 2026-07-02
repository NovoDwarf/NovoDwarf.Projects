using System.Numerics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Circles;

public class WuCircleRasterizer : ICircleRasterizer
{
	public static WuCircleRasterizer Default { get; } = new();
	
	private WuCircleRasterizer() { }
	
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1)
	{
		float dx = x1 - x0;
		float dy = y1 - y0;
		var radius = MathF.Sqrt(dx * dx + dy * dy);

		var limit = (int)MathF.Ceiling(radius / MathF.Sqrt(2f));

		for (var x = 0; x <= limit; x++)
		{
			var y = MathF.Sqrt(radius * radius - x * x);

			var yi = (int)MathF.Floor(y);
			var frac = y - yi;
            
			foreach (var p in Plot8(x0, y0, x, yi, 1f - frac))
				yield return p;

			foreach (var p in Plot8(x0, y0, x, yi + 1, frac))
				yield return p;
		}
	}
    
	private static IEnumerable<Vector3> Plot8(int cx, int cy, int x, int y, float alpha)
	{
		if (alpha <= 0f)
			yield break;

		yield return new Vector3(cx + x, cy + y, alpha);
		yield return new Vector3(cx - x, cy + y, alpha);
		yield return new Vector3(cx + x, cy - y, alpha);
		yield return new Vector3(cx - x, cy - y, alpha);

		if (x == y) 
			yield break;
        
		yield return new Vector3(cx + y, cy + x, alpha);
		yield return new Vector3(cx - y, cy + x, alpha);
		yield return new Vector3(cx + y, cy - x, alpha);
		yield return new Vector3(cx - y, cy - x, alpha);
	}
}