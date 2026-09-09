using Serilog;
using SmartHome.MAX;

namespace SmartHome.Host.Extensions;

public static class MaxExtensions
{
    /// <summary>
    /// Registers MAX bot hosted service and its named HttpClient.
    /// Returns false and logs a warning if Token is not configured.
    /// </summary>
    public static bool AddMaxIntegration(this WebApplicationBuilder builder)
    {
        var token = builder.Configuration["Max:Token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            Log.Warning("MAX integration disabled: Token is not configured");
            return false;
        }

        var opts = new MaxBotOptions
        {
            Token = token,
            BaseUrl = builder.Configuration["Max:BaseUrl"] ?? "https://api.max.ru",
            AllowedUserIds = builder.Configuration
                .GetSection("Max:AllowedUserIds")
                .Get<string[]>() ?? []
        };

        builder.Services.AddHttpClient("max", client =>
        {
            client.BaseAddress = new Uri(opts.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(35);
        });

        builder.Services.AddSingleton(opts);
        builder.Services.AddHostedService<MaxBotService>();

        Log.Information("MAX integration registered → {BaseUrl}", opts.BaseUrl);
        return true;
    }
}
