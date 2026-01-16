using SkiaSharp;

namespace Mathematics.App.Maui.UI.Graphics;

public class GridStyle
{
	public SKPaint GridPaint { get; } = new()
	{
		Color = SKColors.LightGray, 
		StrokeWidth = 1, 
		Style = SKPaintStyle.Stroke
	};
	
	public SKPaint PointPaint { get; } = new()
	{
		Color = 
			SKColors.OrangeRed, 
		Style = SKPaintStyle.Fill
	};
	
	public SKPaint CurrentPaint { get; } = new()
	{
		Color = SKColors.Black, 
		Style = SKPaintStyle.Fill
	};
	
	public SKPaint PrevPaint { get; } = new()
	{
		Color = SKColors.SlateGray, 
		Style = SKPaintStyle.Fill
	};
	
	public SKPaint ReferencePaint { get; } = new()
	{
		Color = SKColors.AliceBlue, 
		StrokeWidth = 1, 
		Style = SKPaintStyle.Stroke
	};
	
	public SKPaint TextPaint { get; } = new()
	{
		Color = SKColors.LightSlateGray, 
		TextSize = 10
	};
}