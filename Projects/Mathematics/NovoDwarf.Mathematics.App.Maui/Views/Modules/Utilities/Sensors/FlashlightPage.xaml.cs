using NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;

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