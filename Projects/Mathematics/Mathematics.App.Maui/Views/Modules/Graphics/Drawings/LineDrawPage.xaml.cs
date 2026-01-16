using System.Numerics;
using Mathematics.App.Maui.UI.Graphics;
using Mathematics.App.Maui.UI.Views.Graphics.Drawings;
using Mathematics.Graphics.Drawings.Lines;
using SkiaSharp;

namespace Mathematics.App.Maui.UI.Views.Graphics;

public partial class LineDrawPage : DrawPageBase
{
    public LineDrawPage()
    {
        InitializeComponent();
    }

    protected override GridCanvas? GetGridCanvas() => GridCanvas;

    private bool _waitingForStartPoint = true;

    private SKPoint? _startPoint;
    private SKPoint? _endPoint;
    
    private void OnCanvasTapped(object sender, TappedEventArgs e)
    {
        var pos = e.GetPosition(GridCanvas);

        if (pos == null) return;

        var gridPointI = GridCanvas.ScreenToGrid(new SKPoint((float)((Point)pos).X, (float)((Point)pos).Y));
        var gridPoint = new SKPoint(gridPointI.X, gridPointI.Y);

        if (_waitingForStartPoint)
        {
            _startPoint = gridPoint;
            _waitingForStartPoint = false;
            GridCanvas.SetStartPoint(gridPoint);
        }
        else
        {
            _endPoint = gridPoint;
            _waitingForStartPoint = true;
            GridCanvas.SetEndPoint(gridPoint);

            if (_startPoint == null || _endPoint == null)
                return;

            GridCanvas.SetReferenceLine((SKPoint)_startPoint, (SKPoint)_endPoint);

            DrawLine();
        }
    }
    
    private void OnDrawLineClicked(object sender, EventArgs e) => DrawLine();

    private void DrawLine()
    {
        if (_startPoint == null || _endPoint == null) return;

        var x0 = (int)((SKPoint)_startPoint).X;
        var y0 = (int)((SKPoint)_startPoint).Y;
        var x1 = (int)((SKPoint)_endPoint).X;
        var y1 = (int)((SKPoint)_endPoint).Y;

        var algorithm = AlgorithmPicker.SelectedItem?.ToString() ?? "DDA";

        var linePoints = algorithm switch
        {
            "Простейший пошаговый" => SimpleLineRasterizer.Default.Rasterize(x0, y0, x1, y1),
            "DDA" => DdaLineRasterizer.Default.Rasterize(x0, y0, x1, y1),
            "Брезенхем" => BresenhamLineRasterizer.Default.Rasterize(x0, y0, x1, y1),
            "Ву" => WuLineRasterizer.Default.Rasterize(x0, y0, x1, y1),
            _ => []
        };

        GridCanvas.PrepareNextGeneration();

        foreach (var p in linePoints)
        {
            GridCanvas.AddPoint(new SKPoint(p.X, p.Y), p.Z);
        }

        GridCanvas.InvalidateSurface();
    }
}