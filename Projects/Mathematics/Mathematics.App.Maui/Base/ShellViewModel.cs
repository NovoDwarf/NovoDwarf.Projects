namespace Mathematics.App.Maui.Base;

public partial class ShellViewModel : BaseViewModel
{
	public string Date => DateTime.Now.ToString("dd.MM.yyyy");
	
	public string Version => "1.0.0";
}