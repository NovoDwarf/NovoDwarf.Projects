using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class DeviceInfoPage : ContentPage
{
	public DeviceInfoPage(DeviceInfoViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}