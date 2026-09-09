using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHome.Maui.Models;
using SmartHome.Maui.Services;

namespace SmartHome.Maui.ViewModels;

public sealed partial class ScenesViewModel : ObservableObject
{
    private readonly SmartHomeApiClient _api;

    [ObservableProperty] private ObservableCollection<AutomationState> _automations = [];
    [ObservableProperty] private ObservableCollection<ScriptState> _scripts = [];
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public ScenesViewModel(SmartHomeApiClient api)
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
            var automationsTask = _api.GetAutomationsAsync();
            var scriptsTask     = _api.GetScriptsAsync();
            await Task.WhenAll(automationsTask, scriptsTask);
            var automations = automationsTask.Result;
            var scripts     = scriptsTask.Result;
            Automations = new ObservableCollection<AutomationState>(automations);
            Scripts = new ObservableCollection<ScriptState>(scripts);
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
    async Task TriggerAsync(AutomationState automation)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.TriggerAutomationAsync(automation.EntityId);
            StatusMessage = $"Запущено: {automation.DisplayName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task ToggleAutomationAsync(AutomationState automation)
    {
        StatusMessage = string.Empty;
        try
        {
            if (automation.IsEnabled)
                await _api.DisableAutomationAsync(automation.EntityId);
            else
                await _api.EnableAutomationAsync(automation.EntityId);

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task RunScriptAsync(ScriptState script)
    {
        StatusMessage = string.Empty;
        try
        {
            await _api.RunScriptAsync(script.EntityId);
            StatusMessage = $"Скрипт запущен: {script.DisplayName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
    }
}
