using NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;

public partial class BarometerPage : ContentPage
{
	private BarometerViewModel ViewModel => BindingContext as BarometerViewModel ?? throw new InvalidOperationException();

	public BarometerPage(BarometerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}

	protected override void OnDisappearing()
	{
		//ViewModel.Stop();

		base.OnDisappearing();
	}
}