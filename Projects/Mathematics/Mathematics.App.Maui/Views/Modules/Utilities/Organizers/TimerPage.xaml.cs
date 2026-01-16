using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class TimerPage : ContentPage
{
	public TimerPage(TimerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}