using System.Net.Http.Headers;
using Serilog;
using SmartHome.Health;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.Shared.Commands;

namespace SmartHome.Host.Extensions;

public static class HomeAssistantExtensions
{
    /// <summary>
    /// Registers Home Assistant client and command router.
    /// Returns false and logs a warning if BaseUrl or Token are not configured.
    /// </summary>
    public static bool AddHomeAssistantIntegration(this WebApplicationBuilder builder)
    {
        var baseUrl = builder.Configuration["HomeAssistant:BaseUrl"];
        var token = builder.Configuration["HomeAssistant:Token"];

        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(token))
        {
            Log.Warning("HomeAssistant integration disabled: BaseUrl or Token is not configured");
            return false;
        }

        builder.Services.AddHttpClient("homeassistant", client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        });

        builder.Services.AddHomeAssistantServices();
        builder.Services.AddHealthSyncService();
        builder.Services.AddSingleton<ICommandRouter, CommandRouter>();

        Log.Information("HomeAssistant integration registered → {BaseUrl}", baseUrl);
        return true;
    }
}
