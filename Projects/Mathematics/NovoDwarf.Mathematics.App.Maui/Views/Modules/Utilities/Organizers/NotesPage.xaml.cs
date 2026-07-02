using NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Organizers;

public partial class NotesPage : ContentPage
{
	public NotesPage(NotesViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}