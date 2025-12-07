using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.Base;
using Mathematics.App.Maui.Factories;
using Mathematics.App.Maui.Services;
using Mathematics.App.Maui.UI.Views.Distributions;
using Mathematics.Core.Base.Entities;

namespace Mathematics.App.Maui.UI.ViewModels;

public partial class DistributionPreViewModel : BaseViewModel
{
	private readonly NavigationService _navigationService;
	private readonly DistributionPageFactory _distributionPageFactory;
	
	public DistributionPreViewModel(Distribution distribution, NavigationService navigationService, DistributionPageFactory distributionPageFactory)
	{
		Distribution = distribution;
		
		_navigationService = navigationService;
		_distributionPageFactory = distributionPageFactory;
	}

	public Distribution Distribution { get; private set; }
	
	public string Name => Distribution.Name;
	public string Description => Distribution.Description;
	
	[RelayCommand]
	private void Open()
	{
		_navigationService.PushAsync(_distributionPageFactory.GetOrCreate(Distribution));
	}
	
	//public string Image => _distribution.Image;
}