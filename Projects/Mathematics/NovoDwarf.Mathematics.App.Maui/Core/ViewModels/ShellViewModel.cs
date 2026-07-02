namespace NovoDwarf.Mathematics.App.Core.ViewModels;

public partial class ShellViewModel : BaseViewModel
{
	public static string Date => DateTime.Now.ToString("dd.MM.yyyy");

	public static string Version => "1.0.0";
}