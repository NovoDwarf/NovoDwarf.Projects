using NovoDwarf.Mathematics.App.Systems.Hosting.Interfaces;

namespace NovoDwarf.Mathematics.App.Views.Modules;

public partial class CompressionsPage : ContentPage, ILightPage
{
	public CompressionsPage()
	{
		InitializeComponent();
	}
	
	public string Route => "Algorithms/Compressions";
}