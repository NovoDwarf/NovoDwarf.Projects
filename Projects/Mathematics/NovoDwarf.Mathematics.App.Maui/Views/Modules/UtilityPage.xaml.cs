using NovoDwarf.Mathematics.App.Systems.Hosting.Interfaces;
using NovoDwarf.Mathematics.App.ViewModels.Utilities;

namespace NovoDwarf.Mathematics.App.Views.Modules;

public partial class UtilityPage : ContentPage, ILightPage
{
	public UtilityPage(UtilityViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
	
	public string Route => "Algorithms/Utility";
	

}