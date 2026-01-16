using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.Systems.Utilities;
using Mathematics.App.Maui.UI.Base;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public partial class SpeedometerViewModel : BaseViewModel
{
    private CancellationTokenSource? _cts;

    private const double MaxSpeed = 200;
    private const double Smoothing = 0.2;

    private double _filteredSpeed;

    [ObservableProperty]
    public partial double Speed { get; set; }

    [ObservableProperty]
    public partial double SpeedPercent { get; set; }

    [ObservableProperty]
    public partial bool IsRunning { get; set; }

    public bool CanStart => !IsRunning;

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task Start()
    {
        if (!await PermissionUtils.EnsurePermissionsAsync<Permissions.LocationWhenInUse>())
            return;

        IsRunning = true;
        NotifyStateChanged();

        _cts = new CancellationTokenSource();

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                await UpdateSpeedAsync(_cts.Token);
                await Task.Delay(1000, _cts.Token);
            }
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            StopInternal();
        }
    }

    [RelayCommand(CanExecute = nameof(IsRunning))]
    private void Stop()
    {
        StopInternal();
    }

    private async Task UpdateSpeedAsync(CancellationToken token)
    {
        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));
        var location = await Geolocation.Default.GetLocationAsync(request, token);

        if (location?.Speed is null)
            return;

        var speedKmh = Math.Max(0, (double)location.Speed * 3.6);

        _filteredSpeed += (speedKmh - _filteredSpeed) * Smoothing;

        Speed = Math.Round(_filteredSpeed, 1);
        SpeedPercent = Math.Clamp(Speed / MaxSpeed, 0, 1);
    }

    private void StopInternal()
    {
        _cts?.Cancel();
        _cts = null;

        IsRunning = false;
        Speed = 0;
        SpeedPercent = 0;
        _filteredSpeed = 0;

        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        StartCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();

        OnPropertyChanged(nameof(CanStart));
    }
}
