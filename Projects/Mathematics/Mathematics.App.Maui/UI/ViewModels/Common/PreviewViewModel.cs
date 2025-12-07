using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.Base;
using Mathematics.App.Maui.Factories;
using Mathematics.App.Maui.Services;
using Microsoft.Extensions.Logging;
using DependencyService = Microsoft.Maui.Controls.DependencyService;

namespace Mathematics.App.Maui.UI.ViewModels.Common;

public partial class PreviewViewModel : BaseViewModel
{

	
	public PreviewViewModel()
	{

	}

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = "SEX";

	public Type? Page { get; set; }
	

}