using Mathematics.Core.Base.Graphics;

namespace Mathematics.Core.Interfaces.Drawings;

public interface ILineClipper
{
	public Rect Rect { get; set; }

	bool Clip(ref float x0, ref float y0, ref float x1, ref float y1);
}