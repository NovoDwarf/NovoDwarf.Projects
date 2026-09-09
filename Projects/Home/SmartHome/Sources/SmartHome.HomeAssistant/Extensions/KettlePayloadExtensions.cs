using SmartHome.Core;

namespace SmartHome.HomeAssistant.Extensions;

public static class KettlePayloadExtensions
{
    public static string KettleTemp(this IPayloadCodec codec, string entityId, int temperature)
        => codec.Encode(new BotAction { Verb = "ktemp", EntityId = entityId, Value = temperature });
}
