using System.Numerics;
using Mathematics.App.Maui.UI.Graphics;
using Mathematics.App.Maui.UI.Views.Graphics.Drawings;
using Mathematics.Graphics.Drawings.Circles;
using Mathematics.Graphics.Drawings.Fillers;
using Mathematics.Graphics.Drawings.Lines;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Mathematics.App.Maui.UI.Views.Graphics;

public partial class CirclesDrawPage : DrawPageBase
{
	public CirclesDrawPage()
	{
		InitializeComponent();
    }

	protected override GridCanvas? GetGridCanvas() => GridCanvas;
	
	private bool _waitingForStartPoint = true;

    private SKPoint? _centerPoint;
    private SKPoint? _radiusPoint;
    
    private void OnCanvasTapped(object? sender, TappedEventArgs e)
    {
        var pos = e.GetPosition(GridCanvas);

        if (pos == null) return;

        var gridPointI = GridCanvas.ScreenToGrid(new SKPoint((float)((Point)pos).X, (float)((Point)pos).Y));
        var gridPoint = new SKPoint(gridPointI.X, gridPointI.Y);

        if (_waitingForStartPoint)
        {
            _centerPoint = gridPoint;
            _waitingForStartPoint = false;
            GridCanvas.SetStartPoint(gridPoint);
        }
        else
        {
            _radiusPoint = gridPoint;
            _waitingForStartPoint = true;
            GridCanvas.SetEndPoint(gridPoint);

            if (_centerPoint == null || _radiusPoint == null)
                return;

            GridCanvas.SetReferenceLine((SKPoint)_centerPoint, (SKPoint)_radiusPoint);

            DrawCircle();
        }
    }
    
    private void OnDrawLineClicked(object sender, EventArgs e) => DrawCircle();

    private void DrawCircle()
    {
        if (_centerPoint == null || _radiusPoint == null) return;

        int x0 = (int)((SKPoint)_centerPoint).X;
        int y0 = (int)((SKPoint)_centerPoint).Y;
        int x1 = (int)((SKPoint)_radiusPoint).X;
        int y1 = (int)((SKPoint)_radiusPoint).Y;

        string algorithm = AlgorithmPicker.SelectedItem?.ToString() ?? "DDA";
        var centerPoints = algorithm switch
        {
            "DDA" => DdaCircleRasterizer.Default.Rasterize(x0, y0, x1, y1),
            "Брезенхем" => BresenhamCircleRasterizer.Default.Rasterize(x0, y0, x1, y1),
            "Ву" => WuCircleRasterizer.Default.Rasterize(x0, y0, x1, y1),
            _ => []
        };

        GridCanvas.PrepareNextGeneration();

        var enumerable = centerPoints.ToList();
        
        foreach (var p in enumerable)
            GridCanvas.AddPoint(new SKPoint(p.X, p.Y), p.Z);

        var fillAlgorithm = FillPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(fillAlgorithm))
        {
            var fillPoints = fillAlgorithm switch
            {
                "Flood Fill" => FloodFiller.Default.Fill(
                    x0, y0, (int)GridCanvas.Width, (int)GridCanvas.Height,
                    (x, y, _) => GridCanvas.IsEmpty((int)x, (int)y)
                ),
                "Seed Fill" => SeedFiller.Default.Fill(
                    x0, y0, (int)GridCanvas.Width, (int)GridCanvas.Height,
                    (x, y) => GridCanvas.IsEmpty(x, y)
                ),
                "Scanline Fill" =>
                    ScanlineFiller.Default.Fill(
                        enumerable.Select(p => p with { Z = 1 }).ToList(),
                        (int)GridCanvas.Height,
                        (int)GridCanvas.Width
                    ),
                _ => null
            };

            if (fillPoints != null)
            {
                foreach (var fp in fillPoints)
                    GridCanvas.AddPoint(new SKPoint(fp.X, fp.Y), fp.Z);
            }
        }

        GridCanvas.InvalidateSurface();
    }
}