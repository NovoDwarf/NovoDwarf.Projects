using Mathematics.App.Maui.UI.ViewModels.Algorithms;
using Mathematics.App.Systems.Hosting.Interfaces;

namespace Mathematics.App.Views;

public partial class ProbabilityPage : ContentPage, ILightPage
{
	public ProbabilityPage(DistributionsViewModel viewModel)
	{
		BindingContext = viewModel;

		InitializeComponent();
	}
	
	public string Route => "Algorithms/Distributions";
	

}