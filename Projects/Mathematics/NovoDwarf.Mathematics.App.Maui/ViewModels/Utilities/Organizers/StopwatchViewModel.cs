using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalizationResourceManager.Maui;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Resources.Localizations;
using NovoDwarf.Mathematics.App.Systems.Algorithms.Entities;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Organizers;

public sealed partial class StopwatchViewModel : BaseViewModel
{
    private readonly ILocalizationResourceManager _manager;
    private readonly IDispatcherTimer _timer;

    public StopwatchViewModel(ILocalizationResourceManager manager)
    {
        _manager = manager;

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(10);
        _timer.Tick += OnTick;

        UpdateState();
    }

    public string TimeDisplay => _elapsed.ToString(@"hh\:mm\:ss\.ff");

    public string StartStopText => IsRunning
        ? Components_Resources.ResourceManager.GetString("Button_Stop") ?? ""
        : Components_Resources.ResourceManager.GetString("Button_Start") ?? "";

    public double SecondProgress => _elapsed.TotalSeconds % 60 / 60.0;

    public ObservableCollection<LapItem> Laps { get; } = [];

    [ObservableProperty]
    public partial bool IsRunning { get; set; }

    private TimeSpan _elapsed;
    private TimeSpan _lastLapTime;

    [RelayCommand]
    public void StartStop()
    {
        if (IsRunning)
            _timer.Stop();
        else
            _timer.Start();

        IsRunning = !IsRunning;
        UpdateState();
    }

    [RelayCommand]
    public void Reset()
    {
        if (IsRunning)
            _timer.Stop();

        _elapsed = TimeSpan.Zero;
        _lastLapTime = TimeSpan.Zero;
        Laps.Clear();

        OnPropertyChanged(nameof(TimeDisplay));
        OnPropertyChanged(nameof(SecondProgress));

        if (IsRunning)
            _timer.Start();

        UpdateState();
    }


    [RelayCommand(CanExecute = nameof(IsRunning))]
    public void Lap()
    {
        var delta = _elapsed - _lastLapTime;
        _lastLapTime = _elapsed;

        Laps.Add(new LapItem
        {
            Index = $"{Laps.Count + 1}",
            Time = Format(_elapsed),
            Delta = $"+{Format(delta)}"
        });
    }

    partial void OnIsRunningChanged(bool value)
    {
        LapCommand.NotifyCanExecuteChanged();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _elapsed += _timer.Interval;

        OnPropertyChanged(nameof(TimeDisplay));
        OnPropertyChanged(nameof(SecondProgress));
    }

    private void UpdateState()
    {
        OnPropertyChanged(nameof(StartStopText));
        OnPropertyChanged(nameof(IsRunning));
    }

    private static string Format(TimeSpan time) => time.ToString(@"hh\:mm\:ss\.ff");
}