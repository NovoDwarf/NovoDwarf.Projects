using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

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