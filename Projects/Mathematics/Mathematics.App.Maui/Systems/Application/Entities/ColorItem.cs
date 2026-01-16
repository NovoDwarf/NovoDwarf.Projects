using CommunityToolkit.Mvvm.ComponentModel;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public partial class ColorItem : ObservableObject
{
	public string Name { get; set; } = string.Empty;
	public Color Color { get; set; } = Colors.Black;
	public string Hex { get; set; } = string.Empty;
}