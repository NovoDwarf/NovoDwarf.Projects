using Microsoft.Extensions.DependencyInjection;
using SmartHome.HomeAssistant.Clients.Implementations;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Interfaces;
using SmartHome.HomeAssistant.Utilities;

namespace SmartHome.HomeAssistant.Extensions;

public static class HomeAssistantServiceExtensions
{
    public static IServiceCollection AddHomeAssistantServices(this IServiceCollection services)
    {
        services.AddSingleton<HaHttpContext>();
        services.AddSingleton<IEntityClient>(sp => sp.GetRequiredService<HaHttpContext>());
        services.AddSingleton<IHaStateClient>(sp => sp.GetRequiredService<HaHttpContext>());

        services.AddSingleton<ILightClient, LightService>();
        services.AddSingleton<IHumidifierClient, HumidifierService>();
        services.AddSingleton<IMediaPlayerClient, MediaPlayerService>();
        services.AddSingleton<IKettleClient, KettleService>();
        services.AddSingleton<IScenarioClient, ScenarioService>();
        services.AddSingleton<ISensorClient, SensorService>();

        return services;
    }
}
