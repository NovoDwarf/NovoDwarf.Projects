using Mathematics.App.Systems.Hosting.Interfaces;

namespace Mathematics.App.Views;

public partial class NumericalPage : ContentPage, ILightPage
{
	public NumericalPage()
	{
		InitializeComponent();
	}
	
	public string Route => "Algorithms/Functions";
	
}