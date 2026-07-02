using Mathematics.Core.Base.Graphics;
using Mathematics.Core.Interfaces.Drawings;

namespace Mathematics.Graphics.Drawings.Clippers;

public class CohenSutherlandClipper : ILineClipper
{
    public Rect Rect { get; set; }

    public bool Clip(ref float x0, ref float y0, ref float x1, ref float y1)
    {
        var code0 = ComputeOutCode(x0, y0);
        var code1 = ComputeOutCode(x1, y1);
        var accept = false;

        while (true)
        {
            if ((code0 | code1) == 0)
            {
                accept = true;
                break;
            }

            if ((code0 & code1) != 0)
            {
                break;
            }

            float x = 0, y = 0;
            var outCodeOut = code0 != OutCode.Inside ? code0 : code1;

            if (outCodeOut.HasFlag(OutCode.Top))
            {
                x = x0 + (x1 - x0) * (Rect.MaxY - y0) / (y1 - y0);
                y = Rect.MaxY;
            }
            else if (outCodeOut.HasFlag(OutCode.Bottom))
            {
                x = x0 + (x1 - x0) * (Rect.MinY - y0) / (y1 - y0);
                y = Rect.MinY;
            }
            else if (outCodeOut.HasFlag(OutCode.Right))
            {
                y = y0 + (y1 - y0) * (Rect.MaxX - x0) / (x1 - x0);
                x = Rect.MaxX;
            }
            else if (outCodeOut.HasFlag(OutCode.Left))
            {
                y = y0 + (y1 - y0) * (Rect.MinX - x0) / (x1 - x0);
                x = Rect.MinX;
            }

            if (outCodeOut == code0)
            {
                x0 = x; y0 = y;
                code0 = ComputeOutCode(x0, y0);
            }
            else
            {
                x1 = x; y1 = y;
                code1 = ComputeOutCode(x1, y1);
            }
        }

        return accept;
    }
    
    private OutCode ComputeOutCode(float x, float y)
    {
        var code = OutCode.Inside;
        
        if (x < Rect.MinX) code |= OutCode.Left;
        else if (x > Rect.MaxX) code |= OutCode.Right;
        
        if (y < Rect.MinY) code |= OutCode.Bottom;
        else if (y > Rect.MaxY) code |= OutCode.Top;
        
        return code;
    }
}