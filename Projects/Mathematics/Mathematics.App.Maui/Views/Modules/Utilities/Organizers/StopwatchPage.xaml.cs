using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class StopwatchPage : ContentPage
{
	public StopwatchPage(StopwatchViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}