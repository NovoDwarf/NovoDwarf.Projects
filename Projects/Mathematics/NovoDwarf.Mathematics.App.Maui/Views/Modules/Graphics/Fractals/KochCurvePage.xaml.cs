using System.Numerics;
using Mathematics.Graphics.Fractals.Geometric;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace NovoDwarf.Mathematics.App.Views.Modules.Graphics.Fractals;

public partial class KochCurvePage : ContentPage
{
	public KochCurvePage()
	{
		InitializeComponent();
	}
	
	private float _scale = 1f;
	private Vector2 _offset = Vector2.Zero;

	private readonly KochCurve _koch = KochCurve.Default;

	private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
	{
		var canvas = e.Surface.Canvas;
		canvas.Clear(SKColors.White);

		var paint = new SKPaint
		{
			Color = SKColors.Black,
			IsAntialias = true,
			StrokeWidth = 1
		};

		var start = new Vector2(0, 0);
		var end = new Vector2(800, 0);

		_koch.DrawAdaptive(
			start,
			end,
			WorldToScreen,
			minPixelLength: 0.75f,
			drawLine: (a, b) =>
			{
				var sa = WorldToScreen(a);
				var sb = WorldToScreen(b);

				canvas.DrawLine(
					sa.X, sa.Y,
					sb.X, sb.Y,
					paint);
			});
		
		return;

		Vector2 WorldToScreen(Vector2 p) =>
			new(
				p.X * _scale + _offset.X,
				p.Y * _scale + _offset.Y
			);
	}
	
	private void OnTouch(object sender, SKTouchEventArgs e)
	{
		if (e.ActionType == SKTouchAction.WheelChanged)
		{
			var zoom = e.WheelDelta > 0 ? 1.1f : 0.9f;
			_scale *= zoom;
			Canvas.InvalidateSurface();
		}

		e.Handled = true;
	}
}