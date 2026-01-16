using Mathematics.App.Systems.Hosting.Interfaces;

namespace Mathematics.App.Views;

public partial class CompressionsPage : ContentPage, ILightPage
{
	public CompressionsPage()
	{
		InitializeComponent();
	}
	
	public string Route => "Algorithms/Compressions";
}