using Serilog;
using SmartHome.Telegram;
using Telegram.Bot;

namespace SmartHome.Host.Extensions;

public static class TelegramExtensions
{
    public static bool AddTelegramIntegration(this WebApplicationBuilder builder)
    {
        var token = builder.Configuration["Telegram:Token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            Log.Warning("Telegram integration disabled: [Token] is not configured");
            return false;
        }

        var miniAppUrl = builder.Configuration["Telegram:MiniAppUrl"];

        builder.Services.AddSingleton(new TelegramBotOptions
        {
            Token          = token,
            AllowedChatIds = builder.Configuration
                .GetSection("Telegram:AllowedChatIds")
                .Get<long[]>() ?? [],
            MiniAppUrl     = miniAppUrl,
        });

        builder.Services.AddTelegramServices(new TelegramBotClient(token));

        if (!string.IsNullOrWhiteSpace(miniAppUrl))
            Log.Information("Telegram integration registered [Mini App: {Url}]", miniAppUrl);
        else
            Log.Information("Telegram integration registered [Mini App: not configured]");

        return true;
    }
}
