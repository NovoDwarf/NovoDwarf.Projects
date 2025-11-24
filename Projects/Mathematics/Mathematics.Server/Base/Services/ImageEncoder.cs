using System.Drawing;
using System.Drawing.Imaging;
using Mathematics.Server.Controllers.Noises;

namespace Mathematics.Server.Base.Services;

public class ImageEncoder : IImageEncoder
{
	public byte[] Encode(Bitmap bitmap, ImageFormatType formatType)
	{
		using var stream = new MemoryStream();

		ImageFormat imageFormat = formatType switch
		{
			ImageFormatType.Png  => ImageFormat.Png,
			ImageFormatType.Jpeg => ImageFormat.Jpeg,
			ImageFormatType.Bmp  => ImageFormat.Bmp,
			_ => ImageFormat.Png
		};

		bitmap.Save(stream, imageFormat);
		return stream.ToArray();
	}

	public string GetMimeType(ImageFormatType formatType) =>
		formatType switch
		{
			ImageFormatType.Png  => "image/png",
			ImageFormatType.Jpeg => "image/jpeg",
			ImageFormatType.Bmp  => "image/bmp",
			_ => "image/png"
		};
}