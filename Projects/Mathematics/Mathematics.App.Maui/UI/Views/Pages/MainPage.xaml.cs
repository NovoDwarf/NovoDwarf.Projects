using Mathematics.App.Maui.Services;
using Mathematics.App.Maui.UI.ViewModels;

namespace Mathematics.App.Maui;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		
		BindingContext = new MainPageModel();
        
	}
}