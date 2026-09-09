using Microsoft.Extensions.DependencyInjection;
using SmartHome.Telegram.Handlers;
using Telegram.Bot;

namespace SmartHome.Telegram;

/// <summary>
/// DI registration for the Telegram integration.
/// Lives inside SmartHome.Telegram so that internal types stay internal.
/// </summary>
public static class TelegramServiceExtensions
{
    public static IServiceCollection AddTelegramServices(
        this IServiceCollection services,
        ITelegramBotClient botClient)
    {
        services.AddSingleton(botClient);

        // Domain handlers — add one per HA domain
        services.AddSingleton<IDomainHandler, LightDomainHandler>();
        services.AddSingleton<IDomainHandler, HumidifierDomainHandler>();
        services.AddSingleton<IDomainHandler, MediaDomainHandler>();
        services.AddSingleton<IDomainHandler, KettleDomainHandler>();
        services.AddSingleton<IDomainHandler, ScenarioDomainHandler>();

        services.AddSingleton<TelegramHomeNavigator>();
        services.AddSingleton<TelegramUpdateHandler>();
        services.AddHostedService<TelegramBotService>();

        return services;
    }
}
