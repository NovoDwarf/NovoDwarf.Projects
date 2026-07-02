using NovoDwarf.Mathematics.App.Systems.Application.Constants;
using NovoDwarf.Mathematics.App.Systems.Hosting.Interfaces;
using NovoDwarf.Mathematics.App.ViewModels.Pages;

namespace NovoDwarf.Mathematics.App.Views;

public partial class MainPage : ContentPage, ILightPage
{
	public MainPage(MainViewModel mainViewModel)
	{
		BindingContext = mainViewModel;
		InitializeComponent();
	}

	public string Route => Routes.MainRoute;
}