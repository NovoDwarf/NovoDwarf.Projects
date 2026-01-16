#if WINDOWS
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#endif

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.UI.Base;
using ZXing.Common;
using ZXing.Net.Maui;
using BarcodeFormat = ZXing.BarcodeFormat;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public partial class CodeGeneratorViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial string InputText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SelectedCodeType { get; set; }

    [ObservableProperty]
    public partial ImageSource GeneratedImage { get; set; }

    public string[] CodeTypes { get; } =
    [
	    "QR Code", "DataMatrix", "Aztec", "PDF417", "Code128"
    ];

	public CodeGeneratorViewModel()
	{
		SelectedCodeType = CodeTypes[0];
	}

	[RelayCommand]
	private async Task Generate()
	{
		if (string.IsNullOrWhiteSpace(InputText))
		{
			await Shell.Current.DisplayAlertAsync("Ошибка", "Введите данные для генерации кода", "OK");
			return;
		}

		var format = SelectedCodeType switch
		{
			"QR Code" => BarcodeFormat.QR_CODE,
			"DataMatrix" => BarcodeFormat.DATA_MATRIX,
			"Aztec" => BarcodeFormat.AZTEC,
			"PDF417" => BarcodeFormat.PDF_417,
			"Code128" => BarcodeFormat.CODE_128,
			_ => BarcodeFormat.QR_CODE
		};

		var writer = new BarcodeWriter
		{
			Format = format,
			Options = new EncodingOptions
			{
				Width = 400,
				Height = 400,
				Margin = 1
			}
		};

		var bitmap = writer.Write(InputText);

#if ANDROID
		using var ms = new MemoryStream();
		
		if (Android.Graphics.Bitmap.CompressFormat.Png != null)
			bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);
		
		ms.Position = 0;
		
		GeneratedImage = ImageSource.FromStream(() => new MemoryStream(ms.ToArray()));

#elif IOS
		GeneratedImage = ImageSource.FromStream(() =>
		{
			using var imageData = bitmap.AsPNG();
			return imageData?.AsStream();
		});

#elif WINDOWS
		using var ms = new InMemoryRandomAccessStream();

		var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms);
		var buffer = new byte[bitmap.PixelBuffer.Length];
		
		await bitmap.PixelBuffer.AsStream().ReadExactlyAsync(buffer, 0, buffer.Length);

		encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
			(uint)bitmap.PixelWidth,
			(uint)bitmap.PixelHeight,
			96, 96,
			buffer);

		await encoder.FlushAsync(); // обязательно await

		ms.Seek(0); // или ms.Position = 0;

		GeneratedImage = ImageSource.FromStream(() => ms.AsStream());

#endif

	}
}