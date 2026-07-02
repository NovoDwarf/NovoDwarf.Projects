using SkiaSharp;

namespace NovoDwarf.Mathematics.App.Views.Graphics;

public class GridRenderer
{
    private SKPath? _gridPath;
    private int _cachedCellsX;
    private int _cachedCellsY;

    public void Draw(SKCanvas canvas, GridModel model, Transform2D transform, GridStyle style, int pixelScale)
    {
        canvas.Clear(SKColors.Gray);
        canvas.Save();
        canvas.Translate(transform.PanOffset);

        var view = CalculateViewMetrics(canvas, pixelScale, transform.Zoom);

        DrawGrid(canvas, view, pixelScale, style, transform.Zoom);
        DrawGridLabels(canvas, view, pixelScale, style, transform.Zoom);
        DrawPoints(canvas, model, pixelScale, style, transform.Zoom);
        DrawSelectedPoints(canvas, model, pixelScale, style, transform.Zoom);
        DrawReferenceLine(canvas, model, pixelScale, style, transform.Zoom);

        canvas.Restore();
    }

    private (float Width, float Height, int CellsX, int CellsY) CalculateViewMetrics(SKCanvas canvas, int pixelScale, float zoom)
    {
        var info = canvas.DeviceClipBounds;
        var width = info.Width / zoom;
        var height = info.Height / zoom;
        var cellsX = (int)(width / pixelScale);
        var cellsY = (int)(height / pixelScale);
        return (width, height, cellsX, cellsY);
    }

    private void DrawGrid(SKCanvas canvas, (float Width, float Height, int CellsX, int CellsY) view, int pixelScale, GridStyle style, float zoom)
    {
        if (_gridPath == null || _cachedCellsX != view.CellsX || _cachedCellsY != view.CellsY)
        {
            _gridPath?.Dispose();
            _gridPath = new SKPath();
            for (var x = 0; x <= view.CellsX; x++)
            {
                var px = x * pixelScale * zoom;
                _gridPath.MoveTo(px, 0);
                _gridPath.LineTo(px, view.Height * zoom);
            }
            for (var y = 0; y <= view.CellsY; y++)
            {
                var py = y * pixelScale * zoom;
                _gridPath.MoveTo(0, py);
                _gridPath.LineTo(view.Width * zoom, py);
            }
            _cachedCellsX = view.CellsX;
            _cachedCellsY = view.CellsY;
        }

        var paint = new SKPaint
        {
            Color = style.GridPaint.Color,
            Style = style.GridPaint.Style,
            IsAntialias = style.GridPaint.IsAntialias,
            StrokeWidth = 1f
        };

        canvas.DrawPath(_gridPath, paint);
    }

    private void DrawGridLabels(SKCanvas canvas, (float Width, float Height, int CellsX, int CellsY) view, int pixelScale, GridStyle style, float zoom)
    {
        var step = Math.Max(1, (int)(20 / zoom));
        style.TextPaint.TextSize = 10f / zoom;

        for (var x = 0; x <= view.CellsX; x += step)
        {
            var px = x * pixelScale * zoom;
            canvas.DrawText(x.ToString(), px + 2, 12, style.TextPaint);
        }
        for (var y = 0; y <= view.CellsY; y += step)
        {
            var py = y * pixelScale * zoom;
            canvas.DrawText(y.ToString(), 2, py - 2, style.TextPaint);
        }
    }

    private void DrawPoints(SKCanvas canvas, GridModel model, int pixelScale, GridStyle style, float zoom)
    {
        foreach (var p in model.PrevPoints)
        {
            using var paint = new SKPaint();

            paint.Color = style.PrevPaint.Color.WithAlpha((byte)(p.Intensity * 255));
            paint.Style = style.PrevPaint.Style;
            paint.IsAntialias = style.PrevPaint.IsAntialias;

            canvas.DrawRect(p.Point.X * pixelScale * zoom, p.Point.Y * pixelScale * zoom, pixelScale * zoom, pixelScale * zoom, paint);
        }

        foreach (var p in model.CurrentPoints)
        {
            using var paint = new SKPaint();

            paint.Color = style.CurrentPaint.Color.WithAlpha((byte)(p.Intensity * 255));
            paint.Style = style.CurrentPaint.Style;
            paint.IsAntialias = style.CurrentPaint.IsAntialias;

            canvas.DrawRect(p.Point.X * pixelScale * zoom, p.Point.Y * pixelScale * zoom, pixelScale * zoom, pixelScale * zoom, paint);
        }
    }

    private void DrawSelectedPoints(SKCanvas canvas, GridModel model, int pixelScale, GridStyle style, float zoom)
    {
        if (model.StartPoint is not null)
            DrawPointWithLabel(canvas, (SKPoint)model.StartPoint, "P1", pixelScale, style, zoom);

        if (model.EndPoint is not null)
            DrawPointWithLabel(canvas, (SKPoint)model.EndPoint, "P2", pixelScale, style, zoom);
    }

    private void DrawPointWithLabel(SKCanvas canvas, SKPoint point, string label, int pixelScale, GridStyle style, float zoom)
    {
        var cx = (point.X * pixelScale * zoom) + (pixelScale * zoom / 2f);
        var cy = (point.Y * pixelScale * zoom) + (pixelScale * zoom / 2f);
        var r = Math.Max(3f, 5f); // радиус всегда читаемый на экране

        canvas.DrawCircle(cx, cy, r, style.PointPaint);
        style.TextPaint.TextSize = 12f; // текст фиксированного размера
        canvas.DrawText($"{label} ({point.X},{point.Y})", cx + 6f, cy - 6f, style.TextPaint);
    }

    private void DrawReferenceLine(SKCanvas canvas, GridModel model, int pixelScale, GridStyle style, float zoom)
    {
        if (model.ReferenceStart is null || model.ReferenceEnd is null) 
            return;

        var p1 = (SKPoint)model.ReferenceStart;
        var p2 = (SKPoint)model.ReferenceEnd;

        const float strokeWidth = 1f;

        var paint = new SKPaint
        {
            Color = style.ReferencePaint.Color,
            Style = style.ReferencePaint.Style,
            IsAntialias = style.ReferencePaint.IsAntialias,
            StrokeWidth = strokeWidth
        };

        canvas.DrawLine(
            (p1.X * pixelScale * zoom) + (pixelScale * zoom / 2f),
            (p1.Y * pixelScale * zoom) + (pixelScale * zoom / 2f),
            (p2.X * pixelScale * zoom) + (pixelScale * zoom / 2f),
            (p2.Y * pixelScale * zoom) + (pixelScale * zoom / 2f),
            paint
        );
    }
}