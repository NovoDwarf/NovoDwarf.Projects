namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class NotesPage : ContentPage
{
	public NotesPage(ViewModels.Utilities.NotesViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}