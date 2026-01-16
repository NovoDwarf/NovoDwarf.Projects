using Mathematics.App.Core.Views;
using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.UI.Base;
using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.UI.Views.Utilities.Organizers;

public partial class ConverterPage : MemorablePage
{
	private ConverterViewModel ViewModel => (ConverterViewModel)BindingContext;
	
	public ConverterPage(ConverterViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
	
	public override string Route => Routes.ConverterRoute;
	
	public override void OnLoad(IDictionary<string, object?> parameters) => ViewModel.Load(parameters);

	public override IDictionary<string, object?> OnSave() => ViewModel.Save();
}