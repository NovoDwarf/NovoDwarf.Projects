using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Hosting.Services;
using NovoDwarf.Mathematics.App.Views.Common;

namespace NovoDwarf.Mathematics.App.ViewModels.Common;

public partial class PreviewViewModel : BaseViewModel
{
	private readonly ILogger<PreviewTableView> _logger;

	public PreviewViewModel()
	{
		_logger = InjectionService.Resolve<ILogger<PreviewTableView>>();
	}

	[ObservableProperty]
	public partial string Title { get; set; } = string.Empty;

	[ObservableProperty]
	public partial string Description { get; set; } = string.Empty;

	[ObservableProperty]
	public partial bool IsVisible { get; set; }

	[ObservableProperty]
	public partial ImageSource? Icon { get; set; }

	public Type? View { get; set; }

	public Type? ViewModel { get; set; }

	public object? Payload { get; set; }

	[RelayCommand]
	public async Task Open()
	{
		if (View == null)
		{
			_logger.LogError("View type must be set");
			return;
		}

		try
		{
			var page = ResolvePage(View);

			if (page == null)
				return;

			if (ViewModel != null)
			{
				if (Payload != null)
				{
					var vm = ActivatorUtilities.CreateInstance(MauiProgram.Provider, ViewModel, Payload);

					page.BindingContext = vm;
				}
			}

			await Shell.Current.Navigation.PushAsync(page);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while opening the page");
		}
	}

	private Page? ResolvePage(Type viewType)
	{
		var view = InjectionService.Resolve(viewType);

		if (view is Page page)
			return page;

		_logger.LogError("Resolved view {ViewType} is not a View", viewType.Name);
		
		return null;
	}
}
