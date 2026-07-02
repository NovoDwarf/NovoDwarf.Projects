using System.Reflection;
using NovoDwarf.Mathematics.App.Systems.Application.Services;

namespace NovoDwarf.Mathematics.App.Systems.Application.Handlers;

public static class RoutingHandler
{
	public static void Register(params Assembly[] assemblies)
	{
		var pageTypes = assemblies
			.SelectMany(a => a.GetTypes())
			.Where(t => !t.IsAbstract && typeof(ContentPage).IsAssignableFrom(t) && typeof(INavigable).IsAssignableFrom(t));

		foreach (var pageType in pageTypes)
		{
			var routeProperty = pageType.GetProperty("Route", BindingFlags.Public | BindingFlags.Static);

			if (routeProperty?.GetValue(null) is not string route || string.IsNullOrWhiteSpace(route))
				continue;

			Routing.RegisterRoute(route, pageType);
		}
	}
}