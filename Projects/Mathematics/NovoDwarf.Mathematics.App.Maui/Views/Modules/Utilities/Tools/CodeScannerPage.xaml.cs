using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;

public partial class CodeScannerPage : ContentPage
{
	public CodeScannerPage(CodeScannerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}