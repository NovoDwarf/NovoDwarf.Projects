using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Mathematics.Graphics.Noises.Gradient;

namespace Mathematics.App.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;
	
	public float Scale { get; set; } = 0.003f;
	
	public int Octaves { get; set; } = 8;
	
	public float Persistence { get; set; } = 0.5f;
	
	public float Lacunarity { get; set; } = 2.0f;
	
	public ImageSource? PreviewImage { get; private set; }
	
	public void CreateNoise()
	{
		try
		{
			PreviewImage = GenerateNoiseTexture(1024, 1024);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
	
	private BitmapSource GenerateNoiseTexture(int width, int height)
	{
		var perlin = new PerlinNoise();

		// Создаем массив пикселей для более эффективной работы
		var pixels = new byte[width * height * 4]; // BGRA format
		var index = 0;
		
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var noise = 1;
				var value = (byte)((noise + 1) * 0.5f * 255);
            
				// BGRA format - прямой порядок для BitmapSource
				pixels[index++] = value;     // Blue
				pixels[index++] = value;     // Green
				pixels[index++] = value;     // Red
				pixels[index++] = 255;       // Alpha (непрозрачный)
			}
		}
    
		return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, width * 4); // stride
	}
}