using NovoDwarf.Mathematics.App.ViewModels.Utilities.Sensors;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;

public partial class CompassPage : ContentPage
{
	private CompassViewModel ViewModel => BindingContext as CompassViewModel ?? throw new NullReferenceException();

	public CompassPage(CompassViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
	}
}