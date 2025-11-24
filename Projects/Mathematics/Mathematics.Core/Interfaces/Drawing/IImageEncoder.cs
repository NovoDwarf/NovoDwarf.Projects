using System.Drawing;

namespace Mathematics.Server.Controllers.Noises;

public interface IImageEncoder
{
	byte[] Encode(Bitmap bitmap, ImageFormatType formatType);
	string GetMimeType(ImageFormatType formatType);
}