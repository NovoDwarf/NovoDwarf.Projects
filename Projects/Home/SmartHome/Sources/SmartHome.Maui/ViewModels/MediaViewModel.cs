using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Maui.Models;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class MediaViewModel : ObservableObject
{
    private readonly SmartHomeApiClient _api;

    [ObservableProperty] private ObservableCollection<MediaPlayerState> _players = [];
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public MediaViewModel(SmartHomeApiClient api)
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
            var result = await _api.GetMediaAsync();
            Players = new ObservableCollection<MediaPlayerState>(result);
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
    async Task ToggleAsync(MediaPlayerState player)
    {
        StatusMessage = string.Empty;
        try
        {
            if (player.IsActive)
                await _api.TurnMediaOffAsync(player.EntityId);
            else
                await _api.TurnMediaOnAsync(player.EntityId);

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task PlayAsync(string entityId)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.PlayAsync(entityId);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task PauseAsync(string entityId)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.PauseAsync(entityId);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task SetVolumeAsync((string entityId, int percent) args)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.SetVolumeAsync(args.entityId, args.percent);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task ToggleMuteAsync(MediaPlayerState player)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.SetMuteAsync(player.EntityId, !player.IsMuted);
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }
}
