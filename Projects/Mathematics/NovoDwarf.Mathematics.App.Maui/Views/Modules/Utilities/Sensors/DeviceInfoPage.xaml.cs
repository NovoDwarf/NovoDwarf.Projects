using NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;

public partial class DeviceInfoPage : ContentPage
{
	public DeviceInfoPage(DeviceInfoViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}