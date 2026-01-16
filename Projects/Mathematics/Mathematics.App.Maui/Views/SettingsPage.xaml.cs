using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.Systems.Application.Services;
using Mathematics.App.Maui.UI.ViewModels.Pages;

namespace Mathematics.App.Maui.UI.Views.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}

	public string Route => Routes.SettingsRoute;
}