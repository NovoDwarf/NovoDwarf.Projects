using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Maui.Models;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class KettlesViewModel : ObservableObject
{
    private readonly SmartHomeApiClient _api;

    [ObservableProperty] private ObservableCollection<KettleState> _kettles = [];
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public KettlesViewModel(SmartHomeApiClient api)
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
            var result = await _api.GetKettlesAsync();
            Kettles = new ObservableCollection<KettleState>(result);
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
    async Task ToggleAsync(KettleState kettle)
    {
        StatusMessage = string.Empty;
        try
        {
            if (kettle.IsOn)
                await _api.TurnKettleOffAsync(kettle.EntityId);
            else
                await _api.TurnKettleOnAsync(kettle.EntityId);

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task SetTemperatureAsync((string entityId, int temp) args)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.SetKettleTemperatureAsync(args.entityId, args.temp);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }
}
