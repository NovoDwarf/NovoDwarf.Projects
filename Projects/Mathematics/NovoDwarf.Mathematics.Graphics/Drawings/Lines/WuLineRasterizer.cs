using System.Numerics;
using System.Runtime.CompilerServices;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Lines;

public class WuLineRasterizer : ILineRasterizer
{
    public static WuLineRasterizer Default { get; } = new();
    
    private WuLineRasterizer() { }
    
	public IEnumerable<Vector3> Rasterize(int x0, int y0, int x1, int y1)
    {
        var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        if (steep)
        {
            (x0, y0) = (y0, x0);
            (x1, y1) = (y1, x1);
        }

        if (x0 > x1)
        {
            (x0, x1) = (x1, x0);
            (y0, y1) = (y1, y0);
        }

        float dx = x1 - x0;
        float dy = y1 - y0;
        var gradient = (dx == 0) ? 1 : dy / dx;

        var xEnd = Round(x0);
        var yEnd = y0 + gradient * (xEnd - x0);
        var xGap = 1 - FractionalPart(x0 + 0.5f);
        var xPixel1 = (int)xEnd;
        var yPixel1 = (int)Math.Floor(yEnd);

        if (steep)
        {
            yield return new Vector3(yPixel1, xPixel1, (1 - FractionalPart(yEnd)) * xGap);
            yield return new Vector3(yPixel1 + 1, xPixel1, FractionalPart(yEnd) * xGap);
        }
        else
        {
            yield return new Vector3(xPixel1, yPixel1, (1 - FractionalPart(yEnd)) * xGap);
            yield return new Vector3(xPixel1, yPixel1 + 1, FractionalPart(yEnd) * xGap);
        }

        var intery = yEnd + gradient;

        xEnd = Round(x1);
        yEnd = y1 + gradient * (xEnd - x1);
        xGap = FractionalPart(x1 + 0.5f);
        var xPixel2 = (int)xEnd;
        var yPixel2 = (int)Math.Floor(yEnd);

        if (steep)
        {
            for (var x = xPixel1 + 1; x < xPixel2; x++)
            {
                var y = (int)Math.Floor(intery);
                yield return new Vector3(y, x, 1 - FractionalPart(intery));
                yield return new Vector3(y + 1, x, FractionalPart(intery));
                intery += gradient;
            }

            yield return new Vector3(yPixel2, xPixel2, (1 - FractionalPart(yEnd)) * xGap);
            yield return new Vector3(yPixel2 + 1, xPixel2, FractionalPart(yEnd) * xGap);
        }
        else
        {
            for (var x = xPixel1 + 1; x < xPixel2; x++)
            {
                var y = (int)Math.Floor(intery);
                yield return new Vector3(x, y, 1 - FractionalPart(intery));
                yield return new Vector3(x, y + 1, FractionalPart(intery));
                intery += gradient;
            }

            yield return new Vector3(xPixel2, yPixel2, (1 - FractionalPart(yEnd)) * xGap);
            yield return new Vector3(xPixel2, yPixel2 + 1, FractionalPart(yEnd) * xGap);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FractionalPart(float x) => x - (float)Math.Floor(x);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Round(float x) => (float)Math.Round(x);
}