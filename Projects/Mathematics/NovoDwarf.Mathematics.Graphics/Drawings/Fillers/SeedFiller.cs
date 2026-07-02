using System.Drawing;
using System.Numerics;
using Mathematics.Graphics.Neighborhoods;

namespace Mathematics.Graphics.Drawings.Fillers;

public class SeedFiller
{
	public static SeedFiller Default => new();

	private SeedFiller() { }
	
	public INeighborhood2D Neighborhood { get; set; } = MooreNeighborhood.Default;
	
	public IEnumerable<Vector3> Fill(int startX, int startY, int width, int height, Func<int,int,bool> isTarget)
	{
		var visited = new bool[width, height];
		var stack = new Stack<Point>();
		stack.Push(new Point(startX, startY));

		while (stack.Count > 0)
		{
			var p = stack.Pop();
			var x = p.X;
			var y = p.Y;

			if (x < 0 || x >= width || y < 0 || y >= height) 
				continue;

			if (visited[x, y]) 
				continue;

			if (!isTarget(x, y)) 
				continue;

			var xLeft = x;
			var xRight = x;

			while (xLeft >= 0 && !visited[xLeft, y] && isTarget(xLeft, y))
				xLeft--;
			xLeft++;

			while (xRight < width && !visited[xRight, y] && isTarget(xRight, y))
				xRight++;
			xRight--;

			for (var xi = xLeft; xi <= xRight; xi++)
			{
				visited[xi, y] = true;
				yield return new Vector3(xi, y, 1);
			}

			foreach (var offset in Neighborhood.Offsets2D)
			{
				var ny = y + (int)offset.Y;
				if (ny < 0 || ny >= height) continue;

				for (var xi = xLeft; xi <= xRight; xi++)
				{
					var nx = xi + (int)offset.X;
					if (nx < 0 || nx >= width) continue;

					if (!visited[nx, ny] && isTarget(nx, ny))
						stack.Push(new Point(nx, ny));
				}
			}
		}
	}
}