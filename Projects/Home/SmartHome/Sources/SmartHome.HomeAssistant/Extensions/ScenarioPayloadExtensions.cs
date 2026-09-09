using SmartHome.Core;

namespace SmartHome.HomeAssistant.Extensions;

public static class ScenarioPayloadExtensions
{
    /// <summary>Triggers an automation immediately (even if disabled).</summary>
    public static string TriggerAutomation(this IPayloadCodec codec, string entityId)
        => codec.Encode(new BotAction { Verb = "atrig", EntityId = entityId });
}
