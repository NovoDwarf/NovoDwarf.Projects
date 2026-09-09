using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.Core;

namespace SmartHome.VK;

/// <summary>
/// Encodes/decodes <see cref="BotAction"/> to VK's callback payload JSON format.
/// VK allows up to 255 bytes — using readable short keys.
/// Format: {"a":"verb"[,"id":"entity_id"][,"g":"group"][,"v":int][,"m":"mode"][,"i":int][,"s":int]}
/// </summary>
public sealed class VkPayloadCodec : IPayloadCodec
{
    public static readonly VkPayloadCodec Instance = new();

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public string Encode(BotAction action)
    {
        var dict = new Dictionary<string, object?> { ["a"] = action.Verb };
        if (action.EntityId is not null) dict["id"] = action.EntityId;
        if (action.Group is not null) dict["g"] = action.Group;
        if (action.Value.HasValue) dict["v"] = action.Value;
        if (action.Mode is not null) dict["m"] = action.Mode;
        if (action.Index.HasValue) dict["i"] = action.Index;
        if (action.State.HasValue) dict["s"] = action.State;
        return JsonSerializer.Serialize(dict, SerializerOptions);
    }

    public BotAction? Decode(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            return new BotAction
            {
                Verb = Str(root, "a") ?? "noop",
                EntityId = Str(root, "id"),
                Group = Str(root, "g"),
                Value = Int(root, "v"),
                Mode = Str(root, "m"),
                Index = Int(root, "i"),
                State = Int(root, "s"),
            };
        }
        catch
        {
            return null;
        }
    }

    private static string? Str(JsonElement el, string key)
        => el.TryGetProperty(key, out var p) ? p.GetString() : null;

    private static int? Int(JsonElement el, string key)
        => el.TryGetProperty(key, out var p) && p.ValueKind == JsonValueKind.Number
            ? p.GetInt32()
            : null;
}
