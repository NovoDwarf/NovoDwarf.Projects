using System.Net.Http.Json;
using SmartHome.HomeAssistant.Interfaces;

namespace SmartHome.HomeAssistant.Utilities;

internal sealed class HaHttpContext : IEntityClient, IHaStateClient
{
    private readonly IHttpClientFactory _factory;

    public HaHttpContext(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public HttpClient Http() => _factory.CreateClient("homeassistant");

    public async Task<string> RenderTemplateAsync(string template, CancellationToken ct)
    {
        var response = await Http().PostAsJsonAsync("/api/template", new { template }, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }

    public async Task TurnOnAsync(string entityId, CancellationToken ct = default)
        => await Http().PostAsJsonAsync("/api/services/homeassistant/turn_on", new { entity_id = entityId }, ct);

    public async Task TurnOffAsync(string entityId, CancellationToken ct = default)
        => await Http().PostAsJsonAsync("/api/services/homeassistant/turn_off", new { entity_id = entityId }, ct);

    public async Task SetStateAsync(
        string entityId,
        string state,
        Dictionary<string, object>? attributes = null,
        CancellationToken ct = default)
        => await Http().PostAsJsonAsync(
            $"/api/states/{entityId}",
            new { state, attributes = attributes ?? new Dictionary<string, object>() },
            ct);

    public static string Coalesce(string? value, string fallback)
        => string.IsNullOrWhiteSpace(value) ? fallback : value;
}
