using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Maui.Models;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class HumidifiersViewModel : ObservableObject
{
    private readonly SmartHomeApiClient _api;

    [ObservableProperty] private ObservableCollection<HumidifierState> _humidifiers = [];
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public HumidifiersViewModel(SmartHomeApiClient api)
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
            var result = await _api.GetHumidifiersAsync();
            Humidifiers = new ObservableCollection<HumidifierState>(result);
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
    async Task ToggleAsync(HumidifierState humidifier)
    {
        StatusMessage = string.Empty;
        try
        {
            if (humidifier.IsOn)
                await _api.TurnHumidifierOffAsync(humidifier.EntityId);
            else
                await _api.TurnHumidifierOnAsync(humidifier.EntityId);

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task SetHumidityAsync((string entityId, int value) args)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.SetHumidityAsync(args.entityId, args.value);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task SetModeAsync((string entityId, string mode) args)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.SetHumidifierModeAsync(args.entityId, args.mode);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }
}
