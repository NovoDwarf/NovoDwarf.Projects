using Mathematics.App.Maui.Systems.Application.Constants;
using Mathematics.App.Systems.Hosting.Interfaces;

namespace Mathematics.App.Views;

public partial class ModulesPage : ContentPage, ILightPage
{
	public ModulesPage()
	{
		InitializeComponent();
	}
	
	public string Route => Routes.ModulesRoute;
}