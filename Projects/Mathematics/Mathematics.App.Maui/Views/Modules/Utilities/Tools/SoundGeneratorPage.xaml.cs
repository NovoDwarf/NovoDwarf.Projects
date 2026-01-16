using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class SoundGeneratorPage : ContentPage
{
	public SoundGeneratorPage(SoundGeneratorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}