using System.Net.Http.Json;
using System.Text.Json;
using SmartHome.Maui.Models;

namespace SmartHome.Maui.Services;

public sealed class SmartHomeApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly AppSettings _settings;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public SmartHomeApiClient(IHttpClientFactory factory, AppSettings settings)
    {
        _factory = factory;
        _settings = settings;
    }

    private string Base => $"{_settings.ServerUrl}/api/smarthome";

    private HttpClient CreateClient()
    {
        var client = _factory.CreateClient("smarthome");
        client.DefaultRequestHeaders.Remove("X-Api-Key");
        client.DefaultRequestHeaders.Add("X-Api-Key", _settings.ApiKey);
        return client;
    }
    
    public async Task<IReadOnlyList<LightState>> GetLightsAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<LightState>>($"{Base}/lights", JsonOpts, ct) ?? [];
    }

    public async Task TurnLightOnAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/lights/{Uri.EscapeDataString(entityId)}/on", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task TurnLightOffAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/lights/{Uri.EscapeDataString(entityId)}/off", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task SetBrightnessAsync(string entityId, int brightness, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/lights/{Uri.EscapeDataString(entityId)}/brightness/{brightness}", null, ct)).EnsureSuccessStatusCode();
    }
    
    public async Task<IReadOnlyList<HumidifierState>> GetHumidifiersAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<HumidifierState>>($"{Base}/humidifiers", JsonOpts, ct) ?? [];
    }

    public async Task TurnHumidifierOnAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/humidifiers/{Uri.EscapeDataString(entityId)}/on", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task TurnHumidifierOffAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/humidifiers/{Uri.EscapeDataString(entityId)}/off", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task SetHumidityAsync(string entityId, int value, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/humidifiers/{Uri.EscapeDataString(entityId)}/humidity/{value}", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task SetHumidifierModeAsync(string entityId, string mode, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/humidifiers/{Uri.EscapeDataString(entityId)}/mode/{Uri.EscapeDataString(mode)}", null, ct)).EnsureSuccessStatusCode();
    }
    
    public async Task<IReadOnlyList<KettleState>> GetKettlesAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<KettleState>>($"{Base}/kettles", JsonOpts, ct) ?? [];
    }

    public async Task TurnKettleOnAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/kettles/{Uri.EscapeDataString(entityId)}/on", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task TurnKettleOffAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/kettles/{Uri.EscapeDataString(entityId)}/off", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task SetKettleTemperatureAsync(string entityId, int temp, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/kettles/{Uri.EscapeDataString(entityId)}/temperature/{temp}", null, ct)).EnsureSuccessStatusCode();
    }
    
    public async Task<IReadOnlyList<MediaPlayerState>> GetMediaAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<MediaPlayerState>>($"{Base}/media", JsonOpts, ct) ?? [];
    }

    public async Task TurnMediaOnAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/media/{Uri.EscapeDataString(entityId)}/on", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task TurnMediaOffAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/media/{Uri.EscapeDataString(entityId)}/off", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task PlayAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/media/{Uri.EscapeDataString(entityId)}/play", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task PauseAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/media/{Uri.EscapeDataString(entityId)}/pause", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task SetVolumeAsync(string entityId, int percent, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/media/{Uri.EscapeDataString(entityId)}/volume/{percent}", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task SetMuteAsync(string entityId, bool muted, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/media/{Uri.EscapeDataString(entityId)}/mute/{muted.ToString().ToLowerInvariant()}", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task SelectSourceAsync(string entityId, int idx, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/media/{Uri.EscapeDataString(entityId)}/source/{idx}", null, ct)).EnsureSuccessStatusCode();
    }
    
    public async Task<IReadOnlyList<AutomationState>> GetAutomationsAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<AutomationState>>($"{Base}/automations", JsonOpts, ct) ?? [];
    }

    public async Task TriggerAutomationAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/automations/{Uri.EscapeDataString(entityId)}/trigger", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task EnableAutomationAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/automations/{Uri.EscapeDataString(entityId)}/on", null, ct)).EnsureSuccessStatusCode();
    }

    public async Task DisableAutomationAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/automations/{Uri.EscapeDataString(entityId)}/off", null, ct)).EnsureSuccessStatusCode();
    }
    
    public async Task<IReadOnlyList<ScriptState>> GetScriptsAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<ScriptState>>($"{Base}/scripts", JsonOpts, ct) ?? [];
    }

    public async Task RunScriptAsync(string entityId, CancellationToken ct = default)
    {
        var client = CreateClient();
        (await client.PostAsync($"{Base}/scripts/{Uri.EscapeDataString(entityId)}/run", null, ct)).EnsureSuccessStatusCode();
    }

    // ── Sensors ───────────────────────────────────────────────────────────────

    public async Task<IReadOnlyList<SensorState>> GetSensorsAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<SensorState>>($"{Base}/sensors", JsonOpts, ct) ?? [];
    }

    public async Task<IReadOnlyList<BinarySensorState>> GetBinarySensorsAsync(CancellationToken ct = default)
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<BinarySensorState>>($"{Base}/sensors/binary", JsonOpts, ct) ?? [];
    }
}
