using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace NovoDwarf.Mathematics.App.Views.Graphics;

public partial class GridCanvas : SKCanvasView
{
    public int PixelScale { get; set; } = 8;

    public Transform2D Transform { get; } = new();
    public GridModel Model { get; } = new();
    public GridStyle StyleGrid { get; } = new();

    private readonly GridRenderer _renderer = new();

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        _renderer.Draw(e.Surface.Canvas, Model, Transform, StyleGrid, PixelScale);
    }

    public bool IsEmpty(int x, int y)
    {
        return Model.IsEmpty(x, y);
    }
    
    public void AddPoint(SKPoint point, float intensity = 1f)
    {
        Model.AddPoint(point, intensity);
        InvalidateSurface();
    }

    public void SetStartPoint(SKPoint point)
    {
        Model.StartPoint = point;
        Model.EndPoint = null;
        InvalidateSurface();
    }

    public void SetEndPoint(SKPoint point)
    {
        Model.EndPoint = point;
        InvalidateSurface();
    }

    public void SetReferenceLine(SKPoint start, SKPoint end)
    {
        Model.ReferenceStart = start;
        Model.ReferenceEnd = end;
        InvalidateSurface();
    }

    public void PrepareNextGeneration()
    {
        Model.NextGeneration();
        InvalidateSurface();
    }

    public void SetZoom(float scaleFactor, SKPoint pivotScreen) => Transform.SetZoom(scaleFactor, pivotScreen);
    
    public void Pan(SKPoint delta) => Transform.Pan(delta);
   
    public SKPoint ScreenToWorld(SKPoint screen) => Transform.ScreenToWorld(screen);
    
    public SKPointI ScreenToGrid(SKPoint screen) => new(
        (int)Math.Round(Transform.ScreenToWorld(screen).X / PixelScale),
        (int)Math.Round(Transform.ScreenToWorld(screen).Y / PixelScale)
    );
}