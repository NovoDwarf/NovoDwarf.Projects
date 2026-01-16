using System.Collections.ObjectModel;
using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.Systems.Application.Services;

namespace Mathematics.App.Maui.UI.Views.Pages;

public partial class DebugPage : ContentPage
{
	public ObservableCollection<KeyValuePair<string, string>> PreferencesEntries { get; } 
		= new ObservableCollection<KeyValuePair<string, string>>();
	
	public DebugPage()
	{
		InitializeComponent();
		PreferencesList.ItemsSource = PreferencesEntries;
		LoadPreferences();
	}
	
	public string Route => Routes.DebugRoute;
	
	private void LoadPreferences()
	{
		PreferencesEntries.Clear();

		var keys = new[]
		{
			PreferenceKeys.Theme,
			PreferenceKeys.Locale,
			PreferenceKeys.RecentActivities
		};

		foreach (var key in keys)
		{
			var value = Preferences.Get(key, string.Empty);
			PreferencesEntries.Add(new KeyValuePair<string, string?>(key, value));
		}
	}

	private void OnRefreshClicked(object sender, EventArgs e)
	{
		LoadPreferences();
	}
}