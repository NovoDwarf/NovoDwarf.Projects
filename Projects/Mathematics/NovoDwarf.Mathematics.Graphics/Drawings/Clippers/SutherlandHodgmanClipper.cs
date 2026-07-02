using System.Numerics;
using Mathematics.Core.Base.Graphics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Clippers;

public class SutherlandHodgmanClipper : ILineClipper
{
	public Rect Rect { get; set; }

	public bool Clip(ref float x0, ref float y0, ref float x1, ref float y1)
	{
		var polygon = new List<Vector2>
		{
			new(x0, y0),
			new(x1, y1)
		};

		var clipped = SutherlandHodgmanClipper.Clip(polygon, Rect);

		if (clipped.Count < 2)
			return false;

		x0 = clipped[0].X;
		y0 = clipped[0].Y;
		x1 = clipped[^1].X;
		y1 = clipped[^1].Y;

		return true;
	}
	
	public static List<Vector2> Clip(List<Vector2> polygon, Rect clipRect)
	{
		var outputList = new List<Vector2>(polygon);

		outputList = ClipAgainstEdge(outputList, new Vector2(clipRect.MinX, 0), new Vector2(-1, 0));
		outputList = ClipAgainstEdge(outputList, new Vector2(clipRect.MaxX, 0), new Vector2(1, 0));
		outputList = ClipAgainstEdge(outputList, new Vector2(0, clipRect.MinY), new Vector2(0, -1));
		outputList = ClipAgainstEdge(outputList, new Vector2(0, clipRect.MaxY), new Vector2(0, 1));

		return outputList;
	}

	private static List<Vector2> ClipAgainstEdge(List<Vector2> polygon, Vector2 edgePoint, Vector2 edgeNormal)
	{
		var output = new List<Vector2>();
		
		if (polygon.Count == 0)
			return output;

		var prevPoint = polygon[^1];
		
		foreach (var currPoint in polygon)
		{
			var currInside = IsInside(currPoint, edgePoint, edgeNormal);
			var prevInside = IsInside(prevPoint, edgePoint, edgeNormal);

			if (currInside)
			{
				if (!prevInside)
				{
					var intersect = ComputeIntersection(prevPoint, currPoint, edgePoint, edgeNormal);
					output.Add(intersect);
				}
				output.Add(currPoint);
			}
			else if (prevInside)
			{
				var intersect = ComputeIntersection(prevPoint, currPoint, edgePoint, edgeNormal);
				output.Add(intersect);
			}

			prevPoint = currPoint;
		}

		return output;
	}

	private static bool IsInside(Vector2 point, Vector2 edgePoint, Vector2 edgeNormal)
	{
		var v = point - edgePoint;
		return Vector2.Dot(v, edgeNormal) >= 0;
	}

	private static Vector2 ComputeIntersection(Vector2 p1, Vector2 p2, Vector2 edgePoint, Vector2 edgeNormal)
	{
		var d = p2 - p1;
		var t = Vector2.Dot(edgePoint - p1, edgeNormal) / Vector2.Dot(d, edgeNormal);
		return p1 + t * d;
	}
}