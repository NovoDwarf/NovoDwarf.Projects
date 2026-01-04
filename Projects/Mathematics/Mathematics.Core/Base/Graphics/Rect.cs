using System.Numerics;

namespace Mathematics.Core.Base.Graphics;

public readonly record struct Rect(float MinX, float MinY, float MaxX, float MaxY)
{
	public float Width => MaxX - MinX;
	public float Height => MaxY - MinY;

	public Vector2 TopLeft => new(MinX, MaxY);
	
	public Vector2 TopRight => new(MaxX, MaxY);
	
	public Vector2 BottomLeft => new(MinX, MinY);
	
	public Vector2 BottomRight => new(MaxX, MinY);

	public bool Contains(float x, float y) => x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;

	public bool Contains(Vector2 p) => Contains(p.X, p.Y);
}
