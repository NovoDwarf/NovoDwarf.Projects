using NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Organizers;

public partial class StopwatchPage : ContentPage
{
	public StopwatchPage(StopwatchViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}