using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mathematics.App.Maui.UI.Base;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public sealed partial class TimerViewModel : BaseViewModel
{
    private readonly IDispatcherTimer _timer;
    private TimeSpan _remaining;
    private TimeSpan _initial;

    public TimerViewModel()
    {
        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTick;

        UpdateDisplay();
    }

    // ===== Ввод =====

    [ObservableProperty]
    public partial string InputHours { get; set; } = "0";

    [ObservableProperty]
    public partial string InputMinutes { get; set; } = "0";

    [ObservableProperty]
    public partial string InputSeconds { get; set; } = "0";
    [ObservableProperty]
    public partial bool IsRunning { get; set; }

    public bool CanStart => !IsRunning && ParseInput().TotalSeconds > 0;

    [ObservableProperty]
    public partial string TimeDisplay { get; set; } = "00:00:00";

    public double Progress => _initial.TotalSeconds <= 0 ? 0 : _remaining.TotalSeconds / _initial.TotalSeconds;

    private void OnTick(object? sender, EventArgs e)
    {
        if (_remaining <= TimeSpan.Zero)
        {
            StopInternal();
            _remaining = TimeSpan.Zero;
            UpdateDisplay();
            return;
        }

        _remaining -= TimeSpan.FromSeconds(1);
        UpdateDisplay();
    }

    [RelayCommand]
    private void Start()
    {
        if (_remaining <= TimeSpan.Zero)
        {
            _initial = ParseInput();
            _remaining = _initial;

            if (_remaining <= TimeSpan.Zero)
                return;
        }

        IsRunning = true;
        _timer.Start();
        NotifyStateChanged();
    }

    [RelayCommand]
    private void Stop()
    {
        StopInternal();
    }

    [RelayCommand]
    private void Reset()
    {
        StopInternal();
        _remaining = TimeSpan.Zero;
        _initial = TimeSpan.Zero;
        UpdateDisplay();
    }

    private void StopInternal()
    {
        _timer.Stop();
        IsRunning = false;
        NotifyStateChanged();
    }

    private TimeSpan ParseInput()
    {
        var h = ParseClamped(InputHours, 0, 99);
        var m = ParseClamped(InputMinutes, 0, 59);
        var s = ParseClamped(InputSeconds, 0, 59);

        return new TimeSpan(h, m, s);
    }

    private static int ParseClamped(string value, int min, int max)
    {
        return !int.TryParse(value, out var result) ? min : Math.Clamp(result, min, max);
    }

    private void UpdateDisplay()
    {
        TimeDisplay = _remaining.ToString(@"hh\:mm\:ss");

        OnPropertyChanged(nameof(Progress));
    }

    private void NotifyStateChanged()
    {
        OnPropertyChanged(nameof(CanStart));
    }
}
