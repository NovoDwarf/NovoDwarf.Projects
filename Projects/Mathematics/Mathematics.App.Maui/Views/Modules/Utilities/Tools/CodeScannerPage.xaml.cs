using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class CodeScannerPage : ContentPage
{
	public CodeScannerPage(CodeScannerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}