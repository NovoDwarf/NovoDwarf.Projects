using Serilog;
using SmartHome.VK;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace SmartHome.Host.Extensions;

public static class VkExtensions
{
    public static bool AddVkIntegration(this WebApplicationBuilder builder)
    {
        var token   = builder.Configuration["Vk:Token"];
        var groupId = builder.Configuration["Vk:GroupId"];

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(groupId))
        {
            Log.Warning("VK integration disabled: [Token] or [GroupId] is not configured");
            return false;
        }

        if (!ulong.TryParse(groupId, out var groupIdUlong))
        {
            Log.Warning("VK integration disabled: GroupId [{GroupId}] is not a valid number", groupId);
            return false;
        }

        builder.Services.AddSingleton<IVkApi>(sp =>
        {
            var api = new VkApi(); 
            api.Authorize(new ApiAuthParams { AccessToken = token });
          
            return api;
        });

        builder.Services.AddSingleton(new VkBotOptions
        {
            Token = token,
            GroupId = groupIdUlong,
            AllowedUserIds = builder.Configuration
                .GetSection("Vk:AllowedUserIds")
                .Get<long[]>() ?? []
        });

        builder.Services.AddHostedService<VkBotService>();

        Log.Information("VK integration registered [{GroupId}]", groupIdUlong);
      
        return true;
    }
}
