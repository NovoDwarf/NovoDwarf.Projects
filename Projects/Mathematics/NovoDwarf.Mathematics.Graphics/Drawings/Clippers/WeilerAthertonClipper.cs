using System.Numerics;
using Mathematics.Core.Base.Graphics;

namespace Mathematics.Graphics.Drawings.Clippers;

public class WeilerAthertonClipper
{
	// Основной метод: возвращает отсечённый многоугольник
	public static List<Vector2> Clip(List<Vector2> subject, List<Vector2> clip)
	{
		var subjectList = CreateVertexList(subject);
		var clipList = CreateVertexList(clip);

		FindIntersections(subjectList, clipList);

		var result = GenerateClippedPolygon(subjectList);

		return result;
	}

	private static List<Vertex> CreateVertexList(List<Vector2> polygon)
	{
		var vertices = new List<Vertex>();
		
		Vertex? prev = null;
		Vertex? first = null;

		foreach (var p in polygon)
		{
			var v = new Vertex(p);
			if (prev != null)
			{
				prev.Next = v;
				v.Prev = prev;
			}
			else
			{
				first = v;
			}
			prev = v;
			vertices.Add(v);
		}

		// Замыкаем список
		prev.Next = first;
		first.Prev = prev;

		return vertices;
	}

	private static void FindIntersections(List<Vertex> subject, List<Vertex> clip)
	{
		foreach (var s1 in subject)
		{
			var s2 = s1.Next;
			
			foreach (var c1 in clip)
			{
				var c2 = c1.Next;

				if (!TryIntersect(s1.Position, s2.Position, c1.Position, c2.Position, out var intersection)) 
					continue;
				
				var interSubj = new Vertex(intersection) { IsIntersection = true };
				var interClip = new Vertex(intersection) { IsIntersection = true };
				interSubj.Corresponding = interClip;
				interClip.Corresponding = interSubj;

				InsertVertex(s1, interSubj);
				InsertVertex(c1, interClip);

				// Можно здесь определить вход/выход для IsEntry
			}
		}
	}

	private static void InsertVertex(Vertex start, Vertex toInsert)
	{
		// Простое вставление после start
		toInsert.Next = start.Next;
		toInsert.Prev = start;
		start.Next.Prev = toInsert;
		start.Next = toInsert;
	}

	private static bool TryIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2, out Vector2 intersection)
	{
		intersection = new Vector2();
		
		var A1 = p2.Y - p1.Y;
		var B1 = p1.X - p2.X;
		var C1 = A1 * p1.X + B1 * p1.Y;

		var A2 = q2.Y - q1.Y;
		var B2 = q1.X - q2.X;
		var C2 = A2 * q1.X + B2 * q1.Y;

		var det = A1 * B2 - A2 * B1;
		
		if (Math.Abs(det) < 1e-6)
			return false;

		intersection.X = (B2 * C1 - B1 * C2) / det;
		intersection.Y = (A1 * C2 - A2 * C1) / det;

		return IsBetween(p1, p2, intersection) && IsBetween(q1, q2, intersection);
	}

	private static bool IsBetween(Vector2 a, Vector2 b, Vector2 c)
	{
		return (c.X >= Math.Min(a.X, b.X) - 1e-6 && c.X <= Math.Max(a.X, b.X) + 1e-6) &&
		       (c.Y >= Math.Min(a.Y, b.Y) - 1e-6 && c.Y <= Math.Max(a.Y, b.Y) + 1e-6);
	}

	private static List<Vector2> GenerateClippedPolygon(List<Vertex> subject)
	{
		var result = new List<Vector2>();

		foreach (var v in subject)
		{
			if (!v.IsIntersection)
				result.Add(v.Position);
			// Для полного алгоритма нужно реализовать обход пересечений с учётом IsEntry/IsExit
		}

		return result;
	}
}