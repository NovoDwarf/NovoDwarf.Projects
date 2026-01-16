using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class CalculatorPage : ContentPage
{
	public CalculatorPage(CalculatorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}