using NovoDwarf.Primitives.Models;

namespace NovoDwarf.Primitives.Extensions;

public static class FieldExtensions
{
	public static Field Fill(int width, int height, float value)
	{
		var field = Field.Rent(width, height);
		
		return field.Fill(width, height, value);
	}
	
	extension(Field field)
	{
		public Field Fill(int width, int height, float value)
		{
			for (var y = 0; y < height; y++)
			for (var x = 0; x < width; x++)
				field[x, y] = value;
		
			return field;
		}

		public float[] ToFloats()
		{
			var w = field.Width;
			var h = field.Height;
		
			var data = new float[w * h];
		
			for (var y = 0; y < h; y++)
			for (var x = 0; x < w; x++)
				data[y * w + x] = field[x, y];
		
			return data;
		}
	}

	public static Field FromFloats(this float[] floats, int width, int height)
	{
		var field = new Field(width, height);
		
		for (var y = 0; y < height; y++)
		for (var x = 0; x < width; x++)
			field[x, y] = floats[y * width + x];
		
		return field;
	}
}