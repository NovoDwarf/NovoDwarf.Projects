using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

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