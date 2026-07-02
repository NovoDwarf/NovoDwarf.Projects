using NovoDwarf.Mathematics.App.Systems.Application.Constants;
using NovoDwarf.Mathematics.App.Systems.Hosting.Interfaces;

namespace NovoDwarf.Mathematics.App.Views;

public partial class ModulesPage : ContentPage, ILightPage
{
	public ModulesPage()
	{
		InitializeComponent();
	}
	
	public string Route => Routes.ModulesRoute;
}