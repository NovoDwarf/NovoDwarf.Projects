using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class HomeViewModel : ObservableObject
{
    private readonly SmartHomeApiClient _api;

    [ObservableProperty] private int _lightsOn;
    [ObservableProperty] private int _lightsTotal;
    [ObservableProperty] private int _humidifiersOn;
    [ObservableProperty] private int _mediaActive;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public string LightsSummary => $"{LightsOn} из {LightsTotal} включено";
    public string HumidifiersSummary => $"{HumidifiersOn} включено";
    public string MediaSummary => $"{MediaActive} активно";

    public HomeViewModel(SmartHomeApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    async Task RefreshAsync()
    {
        IsRefreshing = true;
        StatusMessage = string.Empty;
        try
        {
            var lightsTask       = _api.GetLightsAsync();
            var humidifiersTask  = _api.GetHumidifiersAsync();
            var mediaTask        = _api.GetMediaAsync();
            await Task.WhenAll(lightsTask, humidifiersTask, mediaTask);
            var lights      = lightsTask.Result;
            var humidifiers = humidifiersTask.Result;
            var media       = mediaTask.Result;

            LightsTotal = lights.Count;
            LightsOn = lights.Count(l => l.IsOn);
            HumidifiersOn = humidifiers.Count(h => h.IsOn);
            MediaActive = media.Count(m => m.IsActive);

            OnPropertyChanged(nameof(LightsSummary));
            OnPropertyChanged(nameof(HumidifiersSummary));
            OnPropertyChanged(nameof(MediaSummary));
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}
