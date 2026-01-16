using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class CashCalculatorPage : ContentPage
{
	public CashCalculatorPage(CashCalculatorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}