using NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Organizers;

public partial class TimerPage : ContentPage
{
	public TimerPage(TimerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}