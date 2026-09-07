using Komissar.Core.Enums;
using Komissar.Core.Interfaces;
using Komissar.Diagnostics;
using Komissar.Executions;
using Komissar.Time;
using Microsoft.Extensions.Options;

namespace Komissar;

public sealed class SimulationRuntime<TState> : ISimulationRuntime
{
    private readonly CancellationTokenSource _lifetimeCancellationTokenSource = new();
    private readonly SemaphoreSlim _stepLock = new(1, 1);
    private readonly SimulationOptions _options;
  
    private readonly TState _state;
    private readonly TimeProvider _timeProvider;

    private readonly IReadOnlyList<ISimulationSystem<TState>> _systems;
    private readonly IReadOnlyList<IReadOnlyList<ISimulationSystem<TState>>> _waves;
    private readonly IReadOnlyList<ISimulationObserver<TState>> _observers;
   
    private Task? _runTask;
    private long _tick;

    public SimulationRuntime(
        TState state,
        IOptions<SimulationOptions> options,
        IEnumerable<ISimulationSystem<TState>> systems,
        TimeProvider? timeProvider = null,
        Clock? clock = null,
        TimeScale? timeScale = null,
        IEnumerable<ISimulationObserver<TState>>? observers = null)
        : this(state, options.Value, systems, timeProvider, clock, timeScale, observers)
    {
    }

    public SimulationRuntime(
        TState state,
        SimulationOptions options,
        IEnumerable<ISimulationSystem<TState>> systems,
        TimeProvider? timeProvider = null,
        Clock? clock = null,
        TimeScale? timeScale = null,
        IEnumerable<ISimulationObserver<TState>>? observers = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(options.MaxDegreeOfParallelism, 1);

        _state = state;
        _options = options;
        _systems = [.. systems];
        _waves = CreateWaves(_systems);
        _observers = [.. observers ?? []];
        _timeProvider = timeProvider ?? TimeProvider.System;
       
        Clock = clock ?? new Clock();
        TimeScale = timeScale ?? new TimeScale();
    }

    public SimulationMode Mode => _options.Mode;
    public SimulationStatus Status { get; private set; } = SimulationStatus.Created;

    public long Tick => Interlocked.Read(ref _tick);

    public Clock Clock { get; }
    public TimeScale TimeScale { get; }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Status == SimulationStatus.Running)
        {
            return Task.CompletedTask;
        }

        Status = SimulationStatus.Running;

        if (Mode == SimulationMode.Real)
        {
            _runTask ??= RunAsync(_lifetimeCancellationTokenSource.Token);
        }

        return Task.CompletedTask;
    }

    public Task PauseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (Status == SimulationStatus.Running) 
            Status = SimulationStatus.Paused;

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
       
        await _lifetimeCancellationTokenSource.CancelAsync();

        if (_runTask is not null) 
            await _runTask.ConfigureAwait(false);

        Status = SimulationStatus.Stopped;
    }

    public async ValueTask StepAsync(CancellationToken cancellationToken = default)
    {
        await StepAsync(_options.TickInterval, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask StepAsync(TimeSpan delta, CancellationToken cancellationToken = default)
    {
        if (Status == SimulationStatus.Stopped)
            throw new InvalidOperationException("A stopped simulation cannot execute a step.");

        await _stepLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var tick = Interlocked.Increment(ref _tick);
            Clock.Advance(delta);
            var startedAt = _timeProvider.GetTimestamp();

            for (var waveIndex = 0; waveIndex < _waves.Count; waveIndex++)
            {
                await ExecuteWaveAsync(_waves[waveIndex], waveIndex, tick, delta, cancellationToken).ConfigureAwait(false);
            }

            var metrics = new TickMetrics(
                tick,
                delta,
                _timeProvider.GetElapsedTime(startedAt),
                _systems.Count,
                _waves.Count);

            foreach (var observer in _observers)
                observer.OnTickCompleted(metrics);
        }
        finally
        {
            _stepLock.Release();
        }
    }

    public ValueTask UpdateAsync(TimeSpan realDelta, CancellationToken cancellationToken = default) =>
        StepAsync(TimeScale.Convert(realDelta), cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
      
        _lifetimeCancellationTokenSource.Dispose();
        _stepLock.Dispose();
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_options.TickInterval, _timeProvider);

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                if (Status == SimulationStatus.Running)
                {
                    await UpdateAsync(_options.TickInterval, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private async Task ExecuteWaveAsync(
        IReadOnlyList<ISimulationSystem<TState>> systems,
        int wave,
        long tick,
        TimeSpan delta,
        CancellationToken cancellationToken)
    {
        if (_options.MaxDegreeOfParallelism == 1 || systems.Count == 1)
        {
            foreach (var system in systems)
            {
                var execution = await ExecuteSystemAsync(system, wave, tick, delta, cancellationToken).ConfigureAwait(false);
                execution.Commands.Apply(_state);
            }

            return;
        }

        using var gate = new SemaphoreSlim(_options.MaxDegreeOfParallelism);
        
        var executions = systems.Select(async system =>
        {
            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                return await ExecuteSystemAsync(system, wave, tick, delta, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                gate.Release();
            }
        }).ToArray();

        var results = await Task.WhenAll(executions).ConfigureAwait(false);

        foreach (var execution in results)
            execution.Commands.Apply(_state);
    }

    private Task<SystemExecution> ExecuteSystemAsync(
        ISimulationSystem<TState> system,
        int wave,
        long tick,
        TimeSpan delta,
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            var commands = new CommandBuffer<TState>();
            var context = new SimulationContext<TState>(_state, tick, Clock, delta, _timeProvider.GetUtcNow(), commands);
            var startedAt = _timeProvider.GetTimestamp();

            await system.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);

            var metrics = new SystemMetrics(
                system.GetType().Name,
                tick,
                _timeProvider.GetElapsedTime(startedAt),
                wave);

            foreach (var observer in _observers)
                observer.OnSystemCompleted(metrics);

            return new SystemExecution(commands);
        }, cancellationToken);
    }

    private static IReadOnlyList<IReadOnlyList<ISimulationSystem<TState>>> CreateWaves(
        IReadOnlyList<ISimulationSystem<TState>> systems)
    {
        var waves = new List<List<ISimulationSystem<TState>>>();

        foreach (var system in systems)
        {
            var wave = waves.FirstOrDefault(candidate => candidate.All(existing =>
                system.Descriptor.CanRunInParallelWith(existing.Descriptor)));

            if (wave is null)
            {
                wave = [];
                waves.Add(wave);
            }

            wave.Add(system);
        }

        return waves;
    }

    private sealed record SystemExecution(CommandBuffer<TState> Commands);
}
