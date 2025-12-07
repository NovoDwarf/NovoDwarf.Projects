using Mathematics.App.Maui.UI.ViewModels;

namespace Mathematics.App.Maui.UI.Views.Pages;

public partial class DistributionsPage : ContentPage
{
	public DistributionsPage(DistributionsViewModel viewModel)
	{
		BindingContext = viewModel;
		
		InitializeComponent();
	}
}