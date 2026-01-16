using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.UI.Base;
using ZXing.Net.Maui;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

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