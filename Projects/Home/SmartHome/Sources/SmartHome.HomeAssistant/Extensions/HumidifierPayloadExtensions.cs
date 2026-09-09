using SmartHome.Core;

namespace SmartHome.HomeAssistant.Extensions;

public static class HumidifierPayloadExtensions
{
    extension(IPayloadCodec codec)
    {
        public string SetHumidity(string entityId, int value)
            => codec.Encode(new BotAction { Verb = "hum", EntityId = entityId, Value = value });

        public string SetMode(string entityId, string mode)
            => codec.Encode(new BotAction { Verb = "mode", EntityId = entityId, Mode = mode });
    }
}
