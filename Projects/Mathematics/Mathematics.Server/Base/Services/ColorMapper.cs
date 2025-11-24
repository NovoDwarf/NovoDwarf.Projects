using System.Drawing;
using Mathematics.Server.Controllers.Noises;

namespace Mathematics.Server.Base.Services;

public class ColorMapper : IColorMapper
{
	public Color Map(float v, ColorSchemeType schemeType)
	{
		v = Math.Clamp(v, 0f, 1f);

		return schemeType switch
		{
			ColorSchemeType.Grayscale => Color.FromArgb((int)(v * 255), (int)(v * 255), (int)(v * 255)),
			ColorSchemeType.Inverted  => Color.FromArgb((int)((1f - v) * 255), (int)((1f - v) * 255), (int)((1f - v) * 255)),
			ColorSchemeType.BlueGreen => Color.FromArgb(0, (int)(v * 255), (int)(v * 255)),
			_ => Heatmap(v)
		};
	}

	private Color Heatmap(float x)
	{
		int r = (int)(Math.Clamp(x * 2f, 0f, 1f) * 255);
		int g = (int)(Math.Clamp((x - 0.5f) * 2f, 0f, 1f) * 255);
		int b = (int)((1f - x) * 255);
		return Color.FromArgb(r, g, b);
	}
}