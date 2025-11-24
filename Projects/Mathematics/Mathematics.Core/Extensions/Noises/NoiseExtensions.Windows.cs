using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Mathematics.Core.Interfaces;

namespace Mathematics.Core.Extensions.Noises;

public static partial class NoiseExtensions
{
	[SupportedOSPlatform("windows")]
	public static Bitmap ToHeatmapBitmap(this INoise2D<float> noise, float[,] pixels)
	{
		return ToBitmap(noise, pixels, v => Color.FromArgb(
			(int)(v * 255),
			(int)(v * v * 255),
			(int)((1 - v) * 255)
		));
	}

	[SupportedOSPlatform("windows")]
	public static Bitmap ToGrayscaleBitmap(this INoise2D<float> noise, float[,] pixels)
	{
		return ToBitmap(noise, pixels, v => {
			var c = (int)(v * 255);
			
			return Color.FromArgb(c, c, c);
		});
	}
	
	[SupportedOSPlatform("windows")]
	public static Bitmap ToBitmap(this INoise2D<float> noise, float[,] pixels, Func<float, Color> colorMap)
	{
		var width = pixels.GetLength(0);
		var height = pixels.GetLength(1);

		NormalizeRange(pixels, out var min, out var max);

		var range = max - min;
		
		if (range <= 0.000001f)
			range = 1f;

		var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

		var rect = new Rectangle(0, 0, width, height);
		var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

		var stride = data.Stride;
		var bytes = stride * height;
		var buffer = new byte[bytes];

		for (var y = 0; y < height; y++)
		{
			var rowStart = y * stride;

			for (var x = 0; x < width; x++)
			{
				var v = (pixels[x, y] - min) / range;
				v = Math.Clamp(v, 0f, 1f);
				var color = colorMap(v);

				var idx = rowStart + x * 3;

				buffer[idx + 0] = color.B;
				buffer[idx + 1] = color.G;
				buffer[idx + 2] = color.R;
			}
		}

		Marshal.Copy(buffer, 0, data.Scan0, bytes);
		bmp.UnlockBits(data);

		return bmp;
	}

	private static void NormalizeRange(float[,] pixels, out float min, out float max)
	{
		min = float.MaxValue;
		max = float.MinValue;

		var width = pixels.GetLength(0);
		var height = pixels.GetLength(1);

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var v = pixels[x, y];
				
				if (v < min) min = v;
				if (v > max) max = v;
			}
		}
	}
}
