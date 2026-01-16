using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class SpeedometerPage : ContentPage
{
	public SpeedometerPage(SpeedometerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}