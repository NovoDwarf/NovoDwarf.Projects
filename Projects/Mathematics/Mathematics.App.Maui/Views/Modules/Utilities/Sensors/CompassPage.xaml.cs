using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

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