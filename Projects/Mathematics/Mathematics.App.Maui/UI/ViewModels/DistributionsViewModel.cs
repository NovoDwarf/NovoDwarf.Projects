using System.Collections.ObjectModel;
using Mathematics.App.Maui.Base;
using Mathematics.App.Maui.Factories;
using Mathematics.Core.Base.Entities;
using Mathematics.Distributions.Degenerate;
using Mathematics.Distributions.Univariate.Continuous.Bounded;
using Mathematics.Distributions.Univariate.Continuous.Unbounded;

namespace Mathematics.App.Maui.UI.ViewModels;

public partial class DistributionsViewModel : BaseViewModel
{
	public DistributionsViewModel(DistributionPreViewModelFactory factory)
	{
		Distributions = [];
        
		var distributions = new List<Distribution> 
		{ 
			new DegenerateDistribution(),
			new BetaDistribution(),
			new CauchyDistribution()
		};
        
		foreach (var distribution in distributions)
		{
			Distributions.Add(factory.GetOrCreate(distribution));
		}
	}

	public ObservableCollection<DistributionPreViewModel> Distributions { get; }
}