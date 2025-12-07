namespace Mathematics.App.Maui.Services;

public static class InjectionService
{
	public static T Resolve<T>() where  T : class
	{
		if (Application.Current == null) 
			throw new InvalidOperationException("Current application is null");
		
		var page = Application.Current.Windows[0].Page;
		
		return page?.Handler?.MauiContext?.Services.GetService<T>() ?? throw new InvalidOperationException("Service is not registered");
	}
}