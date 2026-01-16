using SkiaSharp;

namespace Mathematics.App.Maui.UI.Graphics;

public class GridModel
{
    public List<(SKPoint Point, float Intensity)> PrevPoints { get; } = [];
    public List<(SKPoint Point, float Intensity)> CurrentPoints { get; } = [];

	public SKPoint? StartPoint { get; set; }
	public SKPoint? EndPoint { get; set; }
	public SKPoint? ReferenceStart { get; set; }
	public SKPoint? ReferenceEnd { get; set; }

	public bool IsEmpty(int x, int y)
	{
		return !CurrentPoints.Any(p => (int)p.Point.X == x && (int)p.Point.Y == y);
	}
	
	public void AddPoint(SKPoint point, float intensity = 1f)
	{
		CurrentPoints.Add((point, intensity));
	}

	public void NextGeneration()
	{
		PrevPoints.Clear();
		PrevPoints.AddRange(CurrentPoints);
		CurrentPoints.Clear();
	}

	public void Clear()
	{
		PrevPoints.Clear();
		CurrentPoints.Clear();
	}
}
