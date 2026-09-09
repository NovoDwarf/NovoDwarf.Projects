using System;
using System.Diagnostics;
using Avalonia.Threading;
using Komissar;
using Komissar.Core.Enums;
using Komissar.Runtime.Interfaces;

namespace SultanDynasty.Avalonia;

public sealed class AvaloniaSimulationHost : IDisposable
{
	private readonly DispatcherTimer _timer;
	private readonly Stopwatch _stopwatch = new();
	private TimeSpan _lastUpdate;

	public ISimulationRuntime Runtime { get; }

	public bool IsRunning => Runtime.Status == SimulationStatus.Running;
	
	public event EventHandler? Updated;

	public AvaloniaSimulationHost(
		ISimulationRuntime runtime,
		int fps = 60)
	{
		Runtime = runtime;

		_timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(1.0 / fps)
		};

		_timer.Tick += Tick;
	}

	public void Start()
	{
		Runtime.StartAsync().GetAwaiter().GetResult();
		_lastUpdate = TimeSpan.Zero;
		_stopwatch.Restart();

		_timer.Start();
	}

	public void Stop()
	{
		_timer.Stop();

		_stopwatch.Stop();
		Runtime.PauseAsync().GetAwaiter().GetResult();
	}

	private void Tick(object? sender, EventArgs e)
	{
		var now = _stopwatch.Elapsed;
		Runtime.UpdateAsync(now - _lastUpdate).AsTask().GetAwaiter().GetResult();
		_lastUpdate = now;
		Updated?.Invoke(this, EventArgs.Empty);
	}

	public void Step(TimeSpan gameDelta)
	{
		Runtime.StepAsync(gameDelta).AsTask().GetAwaiter().GetResult();
		Updated?.Invoke(this, EventArgs.Empty);
	}

	public void Dispose()
	{
		_timer.Stop();

		_timer.Tick -= Tick;
		_stopwatch.Stop();
	}
}
