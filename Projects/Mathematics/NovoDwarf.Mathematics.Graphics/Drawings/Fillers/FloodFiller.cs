using System.Numerics;
using Mathematics.Graphics.Neighborhoods;

namespace Mathematics.Graphics.Drawings.Fillers;

public class FloodFiller
{
	public static FloodFiller Default => new();
	
	private FloodFiller() { }
	
	public INeighborhood2D Neighborhood { get; set; } = MooreNeighborhood.Default;
    
	public IEnumerable<Vector3> Fill(float startX, float startY, int width, int height, Func<float,float,float,bool> isTarget)
	{
		var visited = new bool[width, height];
		var queue = new Queue<Vector3>();
        
		queue.Enqueue(new Vector3(startX, startY, 1));

		while (queue.Count > 0)
		{
			var p = queue.Dequeue();
			var x = p.X;
			var y = p.Y;

			if (x < 0 || x >= width || y < 0 || y >= height) 
				continue;

			if (visited[(int)x, (int)y]) 
				continue;

			if (!isTarget(x, y, 1)) 
				continue;

			visited[(int)x, (int)y] = true;
			yield return new Vector3(x, y, 1);

			foreach (var offset in Neighborhood.Offsets2D)
			{
				var nx = x + (int)offset.X;
				var ny = y + (int)offset.Y;
                
				queue.Enqueue(new Vector3(nx, ny, 1));
			}
		}
	}
}