using Mathematics.App.Maui.UI.ViewModels;
using Mathematics.Core.Base.Entities;
using Microsoft.Extensions.Logging;

namespace Mathematics.App.Maui.UI.Views.Distributions;

public partial class DistributionView : ContentPage
{
	public DistributionViewModel ViewModel => (DistributionViewModel)BindingContext;
	
	public DistributionView(Distribution distribution, ILogger<DistributionViewModel> logger)
	{
		BindingContext = new DistributionViewModel(distribution, logger);
		
		InitializeComponent();
	}
}