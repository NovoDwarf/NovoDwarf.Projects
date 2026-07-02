using NovoDwarf.Mathematics.App.Systems.Hosting.Interfaces;
using NovoDwarf.Mathematics.App.ViewModels.Probability;

namespace NovoDwarf.Mathematics.App.Views.Modules;

public partial class ProbabilityPage : ContentPage, ILightPage
{
	public ProbabilityPage(DistributionsViewModel viewModel)
	{
		BindingContext = viewModel;

		InitializeComponent();
	}
	
	public string Route => "Algorithms/Distributions";
	

}