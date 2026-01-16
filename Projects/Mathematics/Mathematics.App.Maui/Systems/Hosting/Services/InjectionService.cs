using Serilog;

namespace Mathematics.App.Systems.Hosting.Services;

public sealed class InjectionService
{
	private static IServiceProvider Services => MauiProgram.Provider;

	public static T Resolve<T>() where T : class
	{
		try
		{
			var instance = Services.GetRequiredService<T>();
			return instance;
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Could not resolve dependency [{Type}]", typeof(T).Name);
			throw;
		}
	}

	public static object Resolve(Type type)
	{
		try
		{
			var instance = Services.GetRequiredService(type);
			return instance;
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "Could not resolve dependency [{Type}]", type);
			throw;
		}
	}
}