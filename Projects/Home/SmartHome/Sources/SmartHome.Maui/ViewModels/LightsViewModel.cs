using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Maui.Models;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class LightsViewModel : ObservableObject
{
    private readonly SmartHomeApiClient _api;

    [ObservableProperty] private ObservableCollection<LightState> _lights = [];
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public LightsViewModel(SmartHomeApiClient api)
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
            var result = await _api.GetLightsAsync();
            Lights = new ObservableCollection<LightState>(result);
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

    [RelayCommand]
    async Task ToggleAsync(LightState light)
    {
        StatusMessage = string.Empty;
        try
        {
            if (light.IsOn)
                await _api.TurnLightOffAsync(light.EntityId);
            else
                await _api.TurnLightOnAsync(light.EntityId);

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task SetBrightnessAsync((string entityId, int value) args)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.SetBrightnessAsync(args.entityId, args.value);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }
}
