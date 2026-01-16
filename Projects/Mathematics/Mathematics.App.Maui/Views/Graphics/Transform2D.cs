using SkiaSharp;

namespace Mathematics.App.Maui.UI.Graphics;

public class Transform2D
{
    public float Zoom { get; private set; } = 1f;

    public SKPoint PanOffset { get; private set; } = SKPoint.Empty;

    public float PixelSize { get; set; } = 1f;

    private const float MinZoom = 0.2f;
    private const float MaxZoom = 10f;

    public void SetZoom(float scaleFactor, SKPoint pivotScreen)
    {
        if (scaleFactor <= 0) return;

        var newZoom = Math.Clamp(Zoom * scaleFactor, MinZoom, MaxZoom);
        var worldBefore = ScreenToWorld(pivotScreen);

        Zoom = newZoom;

        var worldAfter = ScreenToWorld(pivotScreen);

        var delta = new SKPoint(
            (worldAfter.X - worldBefore.X) * Zoom,
            (worldAfter.Y - worldBefore.Y) * Zoom
        );

        PanOffset += delta;
    }

    public void Pan(SKPoint delta)
    {
        PanOffset += delta;
    }

    public SKPoint ScreenToWorld(SKPoint screen)
    {
        return new SKPoint(
            (screen.X - PanOffset.X) / Zoom,
            (screen.Y - PanOffset.Y) / Zoom
        );
    }

    public SKPoint WorldToScreen(SKPoint world)
    {
        return new SKPoint(
            world.X * Zoom + PanOffset.X,
            world.Y * Zoom + PanOffset.Y
        );
    }

    public SKPointI ScreenToGrid(SKPoint screen)
    {
        var world = ScreenToWorld(screen);
        return new SKPointI(
            (int)Math.Round(world.X / PixelSize),
            (int)Math.Round(world.Y / PixelSize)
        );
    }
    
    public SKPoint GridToScreen(SKPointI grid)
    {
        var world = new SKPoint(grid.X * PixelSize, grid.Y * PixelSize);
        return WorldToScreen(world);
    }
}
