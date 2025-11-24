using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Mathematics.Core.Interfaces;
using Mathematics.Server.Controllers.Noises;

namespace Mathematics.Core.Extensions.Noises;

public static partial class NoiseExtensions
{
	[SupportedOSPlatform("windows")]

	public static Bitmap ToBitmap(this float[,] pixels, IColorMapper mapper, ColorSchemeType scheme)
	{
		var width = pixels.GetLength(0);
		var height = pixels.GetLength(1);

		var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

		var rect = new Rectangle(0, 0, width, height);
		var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

		var bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
		var stride = bmpData.Stride;
		var buffer = new byte[stride * height];

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var color = mapper.Map(pixels[x, y], scheme);
				var idx = y * stride + x * bytesPerPixel;

				buffer[idx + 0] = color.B; // B
				buffer[idx + 1] = color.G; // G
				buffer[idx + 2] = color.R; // R
				buffer[idx + 3] = color.A; // A
			}
		}

		Marshal.Copy(buffer, 0, bmpData.Scan0, buffer.Length);
		bmp.UnlockBits(bmpData);

		return bmp;
	}
}
