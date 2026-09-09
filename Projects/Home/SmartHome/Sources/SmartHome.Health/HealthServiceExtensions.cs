using Microsoft.Extensions.DependencyInjection;

namespace SmartHome.Health;

public static class HealthServiceExtensions
{
    public static IServiceCollection AddHealthSyncService(this IServiceCollection services)
    {
        services.AddSingleton<IHealthSyncService, HaHealthSyncService>();
        return services;
    }
}
