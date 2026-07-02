using NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;

public partial class AccelerometerPage : ContentPage
{
	private AccelerometerViewModel ViewModel => BindingContext as AccelerometerViewModel ?? throw new InvalidOperationException();

	public AccelerometerPage(AccelerometerViewModel viewModel)
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