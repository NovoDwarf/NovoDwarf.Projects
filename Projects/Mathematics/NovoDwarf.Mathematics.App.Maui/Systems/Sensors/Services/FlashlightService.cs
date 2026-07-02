using NovoDwarf.Mathematics.App.Systems.Sensors.Enums;

namespace NovoDwarf.Mathematics.App.Systems.Sensors.Services;

public sealed class FlashlightService
{
	private CancellationTokenSource? _cts;

	public async Task StartAsync(FlashlightModeType mode, int strobeSpeed, CancellationToken token)
	{
		StopInternal();

		_cts = CancellationTokenSource.CreateLinkedTokenSource(token);
		var ct = _cts.Token;

		switch (mode)
		{
			case FlashlightModeType.Constant:
				await Flashlight.Default.TurnOnAsync();
				break;

			case FlashlightModeType.Strobe:
				_ = RunStrobeAsync(strobeSpeed, ct);
				break;

			case FlashlightModeType.Sos:
				_ = RunSosAsync(ct);
				break;

			case FlashlightModeType.Screen:
				break;
		}
	}

	public async Task StopAsync()
	{
		StopInternal();
		await Flashlight.Default.TurnOffAsync();
	}

	private void StopInternal()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;
	}

	private async Task RunStrobeAsync(int speed, CancellationToken ct)
	{
		try
		{
			while (!ct.IsCancellationRequested)
			{
				await Flashlight.Default.TurnOnAsync();
				await Task.Delay(speed / 2, ct);
				await Flashlight.Default.TurnOffAsync();
				await Task.Delay(speed / 2, ct);
			}
		}
		catch (OperationCanceledException) { }
	}

	private async Task RunSosAsync(CancellationToken ct)
	{
		try
		{
			while (!ct.IsCancellationRequested)
			{
				await BlinkAsync(200, 3, ct);
				await BlinkAsync(600, 3, ct);
				await BlinkAsync(200, 3, ct);
				await Task.Delay(1000, ct);
			}
		}
		catch (OperationCanceledException) { }
	}

	private async Task BlinkAsync(int duration, int count, CancellationToken ct)
	{
		for (var i = 0; i < count; i++)
		{
			await Flashlight.Default.TurnOnAsync();
			await Task.Delay(duration, ct);
			await Flashlight.Default.TurnOffAsync();
			await Task.Delay(200, ct);
		}
	}
}