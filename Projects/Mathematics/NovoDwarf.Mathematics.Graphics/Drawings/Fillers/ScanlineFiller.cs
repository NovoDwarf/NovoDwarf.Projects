using System.Numerics;

namespace Mathematics.Graphics.Drawings.Fillers;

public class ScanlineFiller
{
    public static ScanlineFiller Default { get; } = new();
    
    private ScanlineFiller() { }
    
    public IEnumerable<Vector3> Fill(List<Vector3> polygon, int height, int width)
    {
        var edgeTable = BuildEdgeTable(polygon, height);
        var activeEdges = new List<Edge>();

        for (var y = 0; y < height; y++)
        {
            activeEdges.AddRange(edgeTable[y]);
            activeEdges.RemoveAll(e => e.YMax == y);
            activeEdges.Sort((a, b) => a.X.CompareTo(b.X));

            for (var i = 0; i + 1 < activeEdges.Count; i += 2)
            {
                var xStart = (int)Math.Round(activeEdges[i].X);
                var xEnd = (int)Math.Round(activeEdges[i + 1].X);

                for (var x = xStart; x <= xEnd; x++)
                {
                    if (x >= 0 && x < width)
                        yield return new Vector3(x, y, 1);
                }
            }

            for (var i = 0; i < activeEdges.Count; i++)
            {
                var e = activeEdges[i];
                e.X += e.InvSlope;
                activeEdges[i] = e;
            }
        }
    }
    
    private static List<Edge>[] BuildEdgeTable(List<Vector3> polygon, int height)
    {
        var edgeTable = new List<Edge>[height];
        
        for (var i = 0; i < height; i++)
            edgeTable[i] = [];

        var n = polygon.Count;
        
        for (var i = 0; i < n; i++)
        {
            var point1 = polygon[i];
            var point2 = polygon[(i + 1) % n];

            if ((int)point1.Y == (int)point2.Y)
                continue;

            var top = point1.Y < point2.Y ? point1 : point2;
            var bottom = point1.Y < point2.Y ? point2 : point1;

            var yTop = Math.Clamp((int)top.Y, 0, height - 1);
            var yBottom = Math.Clamp((int)bottom.Y, 0, height - 1);

            if (yTop >= height || yTop < 0 || yBottom >= height)
                continue;

            var invSlope = (bottom.X - top.X) / (bottom.Y - top.Y);
            edgeTable[yTop].Add(new Edge(top.X, invSlope, yBottom));
        }

        return edgeTable;
    }

    
    private record struct Edge(float X, float InvSlope, int YMax);
}