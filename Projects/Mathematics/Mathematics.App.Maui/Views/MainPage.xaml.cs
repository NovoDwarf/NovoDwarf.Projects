using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Maui.UI.ViewModels.Pages;
using Mathematics.App.Systems.Hosting.Interfaces;

namespace Mathematics.App.Views;

public partial class MainPage : ContentPage, ILightPage
{
	public MainPage(MainViewModel mainViewModel)
	{
		BindingContext = mainViewModel;
		InitializeComponent();
	}

	public string Route => Routes.MainRoute;
}