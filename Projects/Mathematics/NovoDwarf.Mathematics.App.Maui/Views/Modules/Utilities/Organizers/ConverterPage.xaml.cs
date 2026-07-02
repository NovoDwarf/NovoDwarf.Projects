using NovoDwarf.Mathematics.App.Core.Views;
using NovoDwarf.Mathematics.App.Systems.Application.Constants;
using NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers;

namespace NovoDwarf.Mathematics.App.Views.Modules.Utilities.Organizers;

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