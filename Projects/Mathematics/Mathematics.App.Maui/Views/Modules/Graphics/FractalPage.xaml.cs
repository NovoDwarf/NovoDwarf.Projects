using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Mathematics.App.Maui.UI.Views.Graphics;

public partial class FractalPage : ContentPage
{
    private enum FractalType
    {
        None,
        KochCurve,
        Tree
    }

    private FractalType _currentFractal = FractalType.None;

    public FractalPage()
    {
        InitializeComponent();
    }

    private void OnKochCurveClicked(object sender, EventArgs e)
    {
        _currentFractal = FractalType.KochCurve;
        CanvasView.InvalidateSurface();
    }

    private void OnTreeClicked(object sender, EventArgs e)
    {
        _currentFractal = FractalType.Tree;
        CanvasView.InvalidateSurface();
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        var paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 2
        };

        switch (_currentFractal)
        {
            case FractalType.KochCurve:
                DrawKochCurve(canvas, paint, 50, e.Info.Height / 2, e.Info.Width - 50, e.Info.Height / 2, 4);
                break;
            case FractalType.Tree:
                DrawTree(canvas, paint, e.Info.Width / 2, e.Info.Height - 50, -90, 100, 5);
                break;
        }
    }

    private void DrawKochCurve(SKCanvas canvas, SKPaint paint, float x1, float y1, float x2, float y2, int depth)
    {
        while (true)
        {
            if (depth == 0)
            {
                canvas.DrawLine(x1, y1, x2, y2, paint);
                return;
            }

            float dx = x2 - x1;
            float dy = y2 - y1;

            float xA = x1 + dx / 3;
            float yA = y1 + dy / 3;

            float xB = x1 + dx * 2 / 3;
            float yB = y1 + dy * 2 / 3;

            float mx = (x1 + x2) / 2;
            float my = (y1 + y2) / 2;

            float xC = (float)(0.5 * (x1 + x2) - Math.Sqrt(3) / 6 * (y2 - y1));
            float yC = (float)(0.5 * (y1 + y2) + Math.Sqrt(3) / 6 * (x2 - x1));

            DrawKochCurve(canvas, paint, x1, y1, xA, yA, depth - 1);
            DrawKochCurve(canvas, paint, xA, yA, xC, yC, depth - 1);
            DrawKochCurve(canvas, paint, xC, yC, xB, yB, depth - 1);
            
            x1 = xB;
            y1 = yB;
            depth -= 1;
        }
    }

    private void DrawTree(SKCanvas canvas, SKPaint paint, float x, float y, double angle, double length, int depth)
    {
        while (true)
        {
            if (depth == 0) 
                return;

            float x2 = x + (float)(length * Math.Cos(angle * Math.PI / 180));
            float y2 = y + (float)(length * Math.Sin(angle * Math.PI / 180));

            canvas.DrawLine(x, y, x2, y2, paint);

            DrawTree(canvas, paint, x2, y2, angle - 20, length * 0.7, depth - 1);
            x = x2;
            y = y2;
            angle = angle + 20;
            length = length * 0.7;
            depth = depth - 1;
        }
    }
}