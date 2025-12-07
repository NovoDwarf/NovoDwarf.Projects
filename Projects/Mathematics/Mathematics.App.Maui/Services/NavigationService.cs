using Microsoft.Extensions.Logging;

namespace Mathematics.App.Maui.Services;

public class NavigationService
{
	private readonly ILogger<NavigationService> _logger;
	
	public NavigationService(ILogger<NavigationService> logger)
	{
		_logger = logger;
	}
	
	public Task InitializeAsync()
	{
		_logger.LogInformation("Инициализация навигации");
		return Task.CompletedTask;
	}

	public Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null)
	{
		_logger.LogInformation("Переход к маршруту {Route}", route);
		return routeParameters != null
				? Shell.Current.GoToAsync(route, routeParameters)
				: Shell.Current.GoToAsync(route);
	}

	public Task PushAsync(Page page)
	{
		_logger.LogInformation("Добавление страницы {Page}", page);
		return Shell.Current.Navigation.PushAsync(page);
	}
	
	public Task PopAsync()
	{
		_logger.LogInformation("Удаление страницы");
		return Shell.Current.Navigation.PopAsync();
	}
}