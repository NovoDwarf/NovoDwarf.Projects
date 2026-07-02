using NovoDwarf.Mathematics.App.Systems.Hosting.Interfaces;

namespace NovoDwarf.Mathematics.App.Views.Modules;

public partial class NumericalPage : ContentPage, ILightPage
{
	public NumericalPage()
	{
		InitializeComponent();
	}
	
	public string Route => "Algorithms/Functions";
	
}