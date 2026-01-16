using Mathematics.App.Maui.UI.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Mathematics.App.Maui.UI.Views.Graphics.Drawings;

public abstract partial class DrawPageBase : ContentPage
{
    protected SKPoint _lastPanPoint;
    protected float _lastPinchScale = 1f;

    protected abstract GridCanvas? GetGridCanvas();

    public virtual void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            _lastPinchScale = 1f;
            return;
        }

        if (e.Status != GestureStatus.Running) return;

        var gridCanvas = GetGridCanvas();
        if (gridCanvas == null) return;

        var screenPivot = new SKPoint(
            (float)(e.ScaleOrigin.X * gridCanvas.Width),
            (float)(e.ScaleOrigin.Y * gridCanvas.Height)
        );

        var scaleDelta = (float)(e.Scale / _lastPinchScale);
        _lastPinchScale = (float)e.Scale;

        gridCanvas.SetZoom(scaleDelta, screenPivot);
        gridCanvas.InvalidateSurface();
    }

    public virtual void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        var gridCanvas = GetGridCanvas();
        if (gridCanvas == null) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _lastPanPoint = new SKPoint((float)e.TotalX, (float)e.TotalY);
                break;

            case GestureStatus.Running:
                var current = new SKPoint((float)e.TotalX, (float)e.TotalY);
                var delta = current - _lastPanPoint;

                gridCanvas.Pan(delta);
                _lastPanPoint = current;
                gridCanvas.InvalidateSurface();
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _lastPanPoint = SKPoint.Empty;
                break;
        }
    }
}

