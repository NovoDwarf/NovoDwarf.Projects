using Microsoft.Extensions.Logging;

namespace Mathematics.App.Maui.Factories;

public interface IPageFactory
{
	Page? Create(Type pageType);
}

public class PageFactory : IPageFactory
{
	private readonly IServiceProvider _services;
	private readonly ILogger<PageFactory> _logger;
	
	public PageFactory(IServiceProvider services, ILogger<PageFactory> logger)
	{
		_services = services;
		_logger = logger;
	}

	public Page? Create(Type pageType)
	{
		try
		{
			if (_services.GetService(pageType) is Page page)
			{
				_logger.LogInformation("Page founded. Returning page.");
				return page;
			}
			else
			{
				_logger.LogWarning("Page didn't found. Returning null.");
				return null;
			}
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "");
		}

		return null;
	}
}