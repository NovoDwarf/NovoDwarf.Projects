namespace NovoDwarf.Primitives.Models;

public sealed record RgbaBitmap(Field R, Field G, Field B, Field A)
{
	public int Width => R.Width;
	public int Height => R.Height;

	public static RgbaBitmap Fill(int width, int height, RgbaColor color)
	{
		color = color.Clamp();
		
		return new RgbaBitmap(
			FillChannel(width, height, color.R),
			FillChannel(width, height, color.G),
			FillChannel(width, height, color.B),
			FillChannel(width, height, color.A));
	}

	private static Field FillChannel(int width, int height, float value)
	{
		var field = new Field(width, height);
		
		for (var y = 0; y < height; y++)
		for (var x = 0; x < width; x++)
			field[x, y] = value;
		
		return field;
	}
}
