using System.Numerics;
using Mathematics.Core.Interfaces.Drawings;
using Mathematics.Graphics.Drawings.Clippers;
using NovoDwarf.Mathematics.App.Views.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using Rect = Mathematics.Core.Base.Graphics.Rect;

namespace NovoDwarf.Mathematics.App.Views.Modules.Graphics.Drawings;

public partial class TruncationDrawPage : DrawPageBase
{
    private readonly GridStyle _gridStyle = new();
    private readonly Rect _clipRect = new(150, 100, 300, 250);
    private readonly List<(Vector2 A, Vector2 B)> _lines =
    [
        (new Vector2(50, 50),   new Vector2(500, 400)),
        (new Vector2(100, 300), new Vector2(450, 120)),
        (new Vector2(200, 50),  new Vector2(200, 500)),
        (new Vector2(20, 200),  new Vector2(600, 200))
    ];
    
    private readonly Transform2D _transform = new() { PixelSize = 5f };
    
	public TruncationDrawPage()
	{
		InitializeComponent();
        
        AlgorithmPicker.SelectedIndex = 0;
        SelectClipper(0);
	}
    
    protected override GridCanvas? GetGridCanvas() => null;
    
    public override void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            _lastPinchScale = 1f;
            return;
        }

        if (e.Status != GestureStatus.Running) return;

        var screenPivot = new SKPoint(
            (float)(e.ScaleOrigin.X * Canvas.Width),
            (float)(e.ScaleOrigin.Y * Canvas.Height)
        );

        var scaleDelta = (float)(e.Scale / _lastPinchScale);
        _lastPinchScale = (float)e.Scale;

        _transform.SetZoom(scaleDelta, screenPivot);
        _cachedCellsX = -1; // Invalidate grid cache when zoom changes
        Canvas.InvalidateSurface();
    }
    
    public override void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _lastPanPoint = new SKPoint((float)e.TotalX, (float)e.TotalY);
                break;

            case GestureStatus.Running:
                var current = new SKPoint((float)e.TotalX, (float)e.TotalY);
                var delta = current - _lastPanPoint;

                _transform.Pan(delta);
                _lastPanPoint = current;
                Canvas.InvalidateSurface();
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _lastPanPoint = SKPoint.Empty;
                break;
        }
    }
    
    private SKPath? _gridPath;
    private int _cachedCellsX = -1;
    private int _cachedCellsY = -1;
	
	private ILineClipper _clipper;
    
    private void OnAlgorithmChanged(object sender, EventArgs e)
    {
        SelectClipper(AlgorithmPicker.SelectedIndex);
        Canvas.InvalidateSurface();
    }

    private Vector2 ToScreen(Vector2 p)
    {
        return new Vector2(p.X * _transform.PixelSize, p.Y * _transform.PixelSize);
    }
    
    private (float Width, float Height, int CellsX, int CellsY) GetView(SKImageInfo info)
    {
        var worldWidth  = info.Width  / _transform.Zoom / _transform.PixelSize;
        var worldHeight = info.Height / _transform.Zoom / _transform.PixelSize;

        return (
            Width: worldWidth,
            Height: worldHeight,
            CellsX: (int)worldWidth,
            CellsY: (int)worldHeight
        );
    }

    
    private void SelectClipper(int index)
    {
        _clipper = index switch
        {
            0 => new CohenSutherlandClipper(),
            1 => new CyrusBeckClipper(),
            2 => new LiangBarskyClipper(),
            3 => new SutherlandHodgmanClipper()
        };

        _clipper.Rect = _clipRect;
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        // Apply transformation matrix
        var matrix = SKMatrix.CreateScale(_transform.Zoom, _transform.Zoom);
        matrix = matrix.PostConcat(SKMatrix.CreateTranslation(_transform.PanOffset.X, _transform.PanOffset.Y));
        canvas.SetMatrix(matrix);

        var view = GetView(e.Info);

        DrawGrid(canvas, view);
        DrawClipRect(canvas);
        DrawLines(canvas);
    }


    private void DrawGrid(SKCanvas canvas, (float Width, float Height, int CellsX, int CellsY) view)
    {
        if (_gridPath == null ||
            _cachedCellsX != view.CellsX ||
            _cachedCellsY != view.CellsY)
        {
            _gridPath?.Dispose();
            _gridPath = new SKPath();

            for (var x = 0; x <= view.CellsX; x++)
            {
                var px = x * _transform.PixelSize;
                _gridPath.MoveTo(px, 0);
                _gridPath.LineTo(px, view.Height * _transform.PixelSize);
            }

            for (var y = 0; y <= view.CellsY; y++)
            {
                var py = y * _transform.PixelSize;
                _gridPath.MoveTo(0, py);
                _gridPath.LineTo(view.Width * _transform.PixelSize, py);
            }

            _cachedCellsX = view.CellsX;
            _cachedCellsY = view.CellsY;
        }

        canvas.DrawPath(_gridPath, _gridStyle.GridPaint);
    }

    
    private void DrawClipRect(SKCanvas canvas)
    {
        // Matrix transformation is applied to canvas, so we just need to multiply by PixelSize
        canvas.DrawRect(
            _clipRect.MinX * _transform.PixelSize,
            _clipRect.MinY * _transform.PixelSize,
            _clipRect.Width * _transform.PixelSize,
            _clipRect.Height * _transform.PixelSize,
            _gridStyle.ReferencePaint
        );
    }

    
    private void DrawLines(SKCanvas canvas)
    {
        foreach (var (a, b) in _lines)
        {
            DrawLine(
                canvas,
                ToScreen(a),
                ToScreen(b),
                _gridStyle.PrevPaint.Color,
                1
            );

            float x0 = a.X, y0 = a.Y;
            float x1 = b.X, y1 = b.Y;

            if (_clipper.Clip(ref x0, ref y0, ref x1, ref y1))
            {
                DrawLine(
                    canvas,
                    ToScreen(new Vector2(x0, y0)),
                    ToScreen(new Vector2(x1, y1)),
                    _gridStyle.PointPaint.Color,
                    3
                );
            }
        }
    }


    private static void DrawLine(
        SKCanvas canvas,
        Vector2 a,
        Vector2 b,
        SKColor color,
        float width)
    {
        using var paint = new SKPaint
        {
            Color = color,
            StrokeWidth = width,
            IsAntialias = true
        };

        canvas.DrawLine(a.X, a.Y, b.X, b.Y, paint);
    }
}