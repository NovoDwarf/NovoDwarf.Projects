using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.Systems.Application.Services;
using Mathematics.App.Maui.UI.Base;
using Mathematics.App.Maui.UI.Views.Utilities;

namespace Mathematics.App.Maui.UI.ViewModels.Pages;

public partial class MainViewModel : BaseViewModel
{
	private readonly ActivityService _activityService;
	
	public MainViewModel(IThemeService themeService, ActivityService activityService)
	{
		_activityService = activityService; 
		
		var recents = activityService.GetAll(ActivityType.Recent);
		var popular = activityService.GetAll(ActivityType.Popular);
		var featured = activityService.GetAll(ActivityType.Featured);
	}

	public ObservableCollection<ActivityItem> RecentActivities { get; set; }
	
	public ObservableCollection<ActivityItem> PopularActivities { get; set; }
	
	public ObservableCollection<ActivityItem> FeaturedActivities { get; set; }

	private void Click(ActivityItem item)
	{
		
	}
	
	private IEnumerable<ActivityItem> CreateActivityView(IReadOnlyList<ActivityEntry> activities)
	{
		foreach (var activityEntry in activities)
		{
			yield return new ActivityItem()
			{

			};
		}
	}
}


public partial class ActivityItem : BaseViewModel
{
	public string Title { get; set; } = string.Empty;
	public string Subtitle { get; set; } = string.Empty;

	public string Route { get; set; } = string.Empty;
	
	[RelayCommand]
	private async Task Open()
	{
		
	}
}