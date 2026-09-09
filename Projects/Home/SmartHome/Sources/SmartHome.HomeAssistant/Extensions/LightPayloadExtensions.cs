using SmartHome.Core;

namespace SmartHome.HomeAssistant.Extensions;

public static class LightPayloadExtensions
{
    public static string Brightness(this IPayloadCodec codec, string entityId, int value)
        => codec.Encode(new BotAction { Verb = "bri", EntityId = entityId, Value = value });
}
