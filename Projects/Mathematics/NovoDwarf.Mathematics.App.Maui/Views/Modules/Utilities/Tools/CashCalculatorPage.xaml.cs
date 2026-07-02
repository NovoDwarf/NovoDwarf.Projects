using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;

public partial class CashCalculatorPage : ContentPage
{
	public CashCalculatorPage(CashCalculatorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}