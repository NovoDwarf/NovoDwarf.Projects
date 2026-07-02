using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;

public partial class CodeGeneratorPage : ContentView
{
	public CodeGeneratorPage(CodeGeneratorViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
}