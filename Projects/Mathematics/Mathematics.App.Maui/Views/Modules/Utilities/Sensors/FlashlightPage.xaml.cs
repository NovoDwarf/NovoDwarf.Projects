using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class FlashlightPage : ContentPage
{
	public FlashlightViewModel ViewModel => (FlashlightViewModel)BindingContext;

	public FlashlightPage(FlashlightViewModel viewModel)
	{
		BindingContext = viewModel;

		InitializeComponent();
	}

	protected override async void OnDisappearing()
	{
		base.OnDisappearing();

		//if (BindingContext is FlashlightViewModel viewModel) 
			//await viewModel.TurnOffAsync();
	}
}