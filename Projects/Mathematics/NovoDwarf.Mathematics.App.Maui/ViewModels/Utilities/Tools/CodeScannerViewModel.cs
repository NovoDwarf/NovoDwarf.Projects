using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using ZXing.Net.Maui;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

public partial class CodeScannerViewModel : BaseViewModel
{
	[ObservableProperty]
	public partial string ResultText { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsDetecting { get; set; } = true;

    [RelayCommand]
	private void BarcodeDetected(BarcodeResult result)
	{
		if (result == null || string.IsNullOrWhiteSpace(result.Value))
			return;

		ResultText = result.Value;
		IsDetecting = false;
	}

	[RelayCommand]
	private void ToggleScanning()
	{
		IsDetecting = !IsDetecting;
	}
}