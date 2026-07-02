using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;

public partial class CalculatorPage : ContentPage
{
	public CalculatorPage(CalculatorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}