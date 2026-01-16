using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities;

public partial class CodeGeneratorPage : ContentView
{
	public CodeGeneratorPage(CodeGeneratorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}