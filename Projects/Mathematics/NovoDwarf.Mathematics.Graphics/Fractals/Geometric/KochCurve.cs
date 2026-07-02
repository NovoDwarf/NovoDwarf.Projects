using System.Numerics;

namespace Mathematics.Graphics.Fractals.Geometric;

public class KochCurve
{
	public static KochCurve Default { get; } = new();

	private KochCurve() { }

	public float Angle { get; set; } = MathF.PI / 3f;

	public List<(Vector2 A, Vector2 B)> Generate(Vector2 start, Vector2 end, int depth)
	{
		var segments = new List<(Vector2, Vector2)>
		{
			(start, end)
		};

		for (var level = 0; level < depth; level++)
		{
			var next = new List<(Vector2, Vector2)>(segments.Count * 4);

			foreach (var (a, b) in segments)
			{
				var v = b - a;

				var p1 = a + v / 3f;
				var p3 = a + v * 2f / 3f;

				var p2 = p1 + Rotate(v / 3f, Angle);

				next.Add((a,  p1));
				next.Add((p1, p2));
				next.Add((p2, p3));
				next.Add((p3, b));
			}

			segments = next;
		}

		return segments;
	}
	
	public IEnumerable<(Vector2 A, Vector2 B)> GenerateAdaptive(
		Vector2 start,
		Vector2 end,
		Func<Vector2, Vector2> worldToScreen,
		float minPixelLength = 1.0f)
	{
		var stack = new Stack<(Vector2 A, Vector2 B)>();
		stack.Push((start, end));

		while (stack.Count > 0)
		{
			var (a, b) = stack.Pop();

			var sa = worldToScreen(a);
			var sb = worldToScreen(b);

			var screenLength = Vector2.Distance(sa, sb);

			if (screenLength <= minPixelLength)
			{
				yield return (a, b);
				continue;
			}

			// Делим сегмент по правилу Коха
			var v = b - a;

			var p1 = a + v / 3f;
			var p3 = a + v * 2f / 3f;
			var p2 = p1 + Rotate(v / 3f, Angle);

			// Порядок важен для корректного обхода
			stack.Push((p3, b));
			stack.Push((p2, p3));
			stack.Push((p1, p2));
			stack.Push((a,  p1));
		}
	}

	public void DrawAdaptive(
		Vector2 start,
		Vector2 end,
		Func<Vector2, Vector2> worldToScreen,
		float minPixelLength,
		Action<Vector2, Vector2> drawLine)
	{
		// Ограничение глубины стека — страховка
		const int MaxStackSize = 1 << 16;

		Span<(Vector2 A, Vector2 B)> stack =
			stackalloc (Vector2, Vector2)[MaxStackSize];

		int top = 0;
		stack[top++] = (start, end);

		while (top > 0)
		{
			var (a, b) = stack[--top];

			var sa = worldToScreen(a);
			var sb = worldToScreen(b);

			if (Vector2.Distance(sa, sb) <= minPixelLength)
			{
				drawLine(a, b);
				continue;
			}

			var v = b - a;
			var p1 = a + v / 3f;
			var p3 = a + v * 2f / 3f;
			var p2 = p1 + Rotate(v / 3f, Angle);

			// DFS: порядок важен
			stack[top++] = (p3, b);
			stack[top++] = (p2, p3);
			stack[top++] = (p1, p2);
			stack[top++] = (a,  p1);
		}
	}
	
	public IEnumerable<IReadOnlyList<(Vector2 A, Vector2 B)>> GenerateSteps(Vector2 start, Vector2 end, int depth)
	{
		var segments = new List<(Vector2, Vector2)> { (start, end) };

		yield return segments;

		for (var level = 0; level < depth; level++)
		{
			var next = new List<(Vector2, Vector2)>(segments.Count * 4);

			foreach (var (a, b) in segments)
			{
				var v = b - a;

				var p1 = a + v / 3f;
				var p3 = a + v * 2f / 3f;
				var p2 = p1 + Rotate(v / 3f, Angle);

				next.Add((a,  p1));
				next.Add((p1, p2));
				next.Add((p2, p3));
				next.Add((p3, b));
			}

			segments = next;
			yield return segments;
		}
	}
	
	private static Vector2 Rotate(Vector2 v, float angle)
	{
		var cos = MathF.Cos(angle);
		var sin = MathF.Sin(angle);

		return new Vector2(
			v.X * cos - v.Y * sin,
			v.Y * cos + v.X * sin
		);
	}
}