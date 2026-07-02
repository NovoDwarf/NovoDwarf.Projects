using NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;

public partial class SpeedometerPage : ContentPage
{
	public SpeedometerPage(SpeedometerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}