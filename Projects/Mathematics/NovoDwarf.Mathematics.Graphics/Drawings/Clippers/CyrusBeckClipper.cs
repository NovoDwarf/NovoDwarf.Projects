using System.Numerics;
using Mathematics.Core.Base.Graphics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Clippers;

public class CyrusBeckClipper : ILineClipper
{
	public Rect Rect { get; set; }
	
	public bool Clip(ref float x0, ref float y0, ref float x1, ref float y1)
	{
		var line = new LineSegment(
			new Vector2(x0, y0),
			new Vector2(x1, y1)
		);

		if (!CyrusBeckClipper.Clip(line, Rect, out var clipped))
			return false;

		x0 = clipped.Start.X;
		y0 = clipped.Start.Y;
		x1 = clipped.End.X;
		y1 = clipped.End.Y;

		return true;
	}

	private static bool Clip(LineSegment line, Rect clipRect, out LineSegment clippedLine)
	{
		var d = line.End - line.Start;
		var tEnter = 0.0f;
		var tLeave = 1.0f;

		Vector2[] normals =
		[
			new Vector2(-1, 0),
			new Vector2(1, 0),
			new Vector2(0, -1),
			new Vector2(0, 1)
		];

		float[] p = [clipRect.MinX, clipRect.MaxX, clipRect.MinY, clipRect.MaxY];
		Vector2[] q =
		[
			line.Start - new Vector2(clipRect.MinX, 0),
			line.Start - new Vector2(clipRect.MaxX, 0),
			line.Start - new Vector2(0, clipRect.MinY),
			line.Start - new Vector2(0, clipRect.MaxY)
		];

		for (var i = 0; i < 4; i++)
		{
			float numerator, denominator;
			if (i < 2)
			{
				numerator = p[i] - line.Start.X;
				denominator = d.X;
			}
			else
			{
				numerator = p[i] - line.Start.Y;
				denominator = d.Y;
			}

			if (denominator == 0)
			{
				if (!(numerator < 0)) 
					continue;
				
				clippedLine = new LineSegment();
				
				return false;
			}

			var t = numerator / denominator;
			if (denominator < 0)
				tEnter = Math.Max(tEnter, t);
			else
				tLeave = Math.Min(tLeave, t);
		}

		if (tEnter > tLeave)
		{
			clippedLine = new LineSegment();
			return false;
		}

		clippedLine = new LineSegment(line.Start + tEnter * d, line.Start + tLeave * d);
		return true;
	}
}