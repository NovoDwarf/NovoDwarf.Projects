using SmartHome.Core;

namespace SmartHome.HomeAssistant.Extensions;

public static class MediaPlayerPayloadExtensions
{
    public static string Play(this IPayloadCodec codec, string entityId)
        => codec.Encode(new BotAction { Verb = "play", EntityId = entityId });

    public static string Pause(this IPayloadCodec codec, string entityId)
        => codec.Encode(new BotAction { Verb = "pause", EntityId = entityId });

    public static string Volume(this IPayloadCodec codec, string entityId, int pct)
        => codec.Encode(new BotAction { Verb = "vol", EntityId = entityId, Value = pct });

    public static string Mute(this IPayloadCodec codec, string entityId, bool muted)
        => codec.Encode(new BotAction { Verb = "mute", EntityId = entityId, State = muted ? 1 : 0 });

    public static string Source(this IPayloadCodec codec, string entityId, int index)
        => codec.Encode(new BotAction { Verb = "src", EntityId = entityId, Index = index });
}
