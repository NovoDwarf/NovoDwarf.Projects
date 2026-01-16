using Mathematics.App.Maui.UI.ViewModels.Algorithms;
using Mathematics.App.Systems.Hosting.Interfaces;
using Mathematics.App.ViewModels.Utilities;

namespace Mathematics.App.Views;

public partial class UtilityPage : ContentPage, ILightPage
{
	public UtilityPage(UtilityViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}
	
	public string Route => "Algorithms/Utility";
	

}