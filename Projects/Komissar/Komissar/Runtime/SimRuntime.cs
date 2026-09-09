using Komissar.Core.Enums;
using Komissar.Diagnostics.Interfaces;
using Komissar.Diagnostics.Metrics;
using Komissar.Runtime.Interfaces;
using Komissar.Systems;
using Komissar.Systems.Interfaces;
using Komissar.Utilities;

namespace Komissar.Runtime;

public sealed class SimRuntime<TState> : ISimRuntime
{
    private readonly CancellationTokenSource _lifetimeCts = new();
    private readonly SemaphoreSlim _stepLock = new(1, 1);
    private readonly Lock _statusLock = new();

    private readonly TState _state;
    
    private readonly ExecutionPlan<TState> _executionPlan;
    private readonly SimExecutor<TState> _executor;

    private readonly SimOptions _options;
    private readonly TimeProvider _timeProvider;

    private readonly IReadOnlyList<ISimulationSystem<TState>> _systems; 
    private readonly IReadOnlyList<ISimObserver<TState>> _observers;
    
    public SimRuntime(
        TState state,
        SimOptions options,
        SystemPlanner<TState> planner,
        IEnumerable<ISimulationSystem<TState>> systems,
        TimeProvider? timeProvider = null,
        SimClock? clock = null,
        TimeScale? timeScale = null,
        IEnumerable<ISimObserver<TState>>? observers = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(systems);

        ArgumentOutOfRangeException.ThrowIfLessThan(options.MaxDegreeOfParallelism, 1);

        if (options.TickInterval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options), "Tick interval must be greater than zero.");

        _state = state;
        _options = options;

        _timeProvider = timeProvider ?? TimeProvider.System;
       
        SimClock = clock ?? new SimClock();
        TimeScale = timeScale ?? new TimeScale();

        _systems = systems.ToList().AsReadOnly();
        _observers = (observers ?? []).ToList().AsReadOnly();

        _executionPlan = planner.Build(_systems);
        _executor = new SimExecutor<TState>(_state, SimClock, _options, _timeProvider, _observers);
    }

    public SimMode Mode => _options.Mode;
    public SimStatus Status { get; private set; } = SimStatus.Created;

    public long Tick => Interlocked.Read(ref _tick);

    public SimClock SimClock { get; }
    public TimeScale TimeScale { get; }

    private Task? _runTask;
    private long _tick;
    
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_statusLock)
        {
            SimpleValidator.EnsureCanStart(Status);

            Status = SimStatus.Running;

            if (Mode == SimMode.Real)
                _runTask ??= RunAsync(_lifetimeCts.Token);
        }

        return Task.CompletedTask;
    }

    public Task PauseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_statusLock)
        {
            if (Status == SimStatus.Running)
                Status = SimStatus.Paused;
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Task? runTask;

        lock (_statusLock)
        {
            if (Status == SimStatus.Stopped)
                return;

            Status = SimStatus.Stopped;
            runTask = _runTask;
        }

        await _lifetimeCts.CancelAsync().ConfigureAwait(false);

        if (runTask is not null)
            await runTask.ConfigureAwait(false);
    }

    public ValueTask StepAsync(CancellationToken cancellationToken = default)
    {
        return StepAsync(_options.TickInterval, cancellationToken);
    }

    public async ValueTask StepAsync(TimeSpan delta, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(delta, TimeSpan.Zero);

        SimpleValidator.EnsureCanStep(Status);

        await _stepLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await ExecuteStepAsync(delta, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _stepLock.Release();
        }
    }

    public ValueTask UpdateAsync(TimeSpan realDelta, CancellationToken cancellationToken = default)
    {
        return StepAsync(TimeScale.Convert(realDelta), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);

        _lifetimeCts.Dispose();
        _stepLock.Dispose();
    }

    private async ValueTask ExecuteStepAsync(TimeSpan delta, CancellationToken cancellationToken)
    {
        var nextTick = Tick + 1;
        var timestamp = _timeProvider.GetUtcNow();
        var startedAt = _timeProvider.GetTimestamp();

        await _executor.ExecuteAsync(_executionPlan, new SimExecContext(nextTick, delta, timestamp), cancellationToken)
                       .ConfigureAwait(false);

        CommitTick(nextTick, delta);

        var metrics = new TickMetrics(
            nextTick,
            delta,
            _timeProvider.GetElapsedTime(startedAt),
            _executionPlan.SystemCount,
            _executionPlan.WaveCount);

        NotifyTickCompleted(metrics);
    }

    private void CommitTick(long tick, TimeSpan delta)
    {
        SimClock.Advance(delta);
        Interlocked.Exchange(ref _tick, tick);
    }
    
    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_options.TickInterval, _timeProvider);

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                if (Status != SimStatus.Running)
                    continue;

                await UpdateAsync(_options.TickInterval, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            
        }
    }

    private void NotifyTickCompleted(TickMetrics metrics)
    {
        foreach (var observer in _observers)
            observer.OnTickCompleted(metrics);
    }
}