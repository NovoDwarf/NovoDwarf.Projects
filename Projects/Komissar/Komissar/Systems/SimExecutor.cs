using Komissar.Core.Enums;
using Komissar.Diagnostics.Interfaces;
using Komissar.Diagnostics.Metrics;
using Komissar.Runtime;
using Komissar.Systems.Interfaces;

namespace Komissar.Systems;

public sealed class SimExecutor<TState>
{
    private readonly TState _state;
    
    private readonly SimClock _simClock;
    private readonly SimOptions _options;
    private readonly TimeProvider _timeProvider;
    
    private readonly IReadOnlyList<ISimObserver<TState>> _observers;

    public SimExecutor(
        TState state,
        SimClock simClock,
        SimOptions options,
        TimeProvider timeProvider,
        IReadOnlyList<ISimObserver<TState>> observers)
    {
        _state = state;
        _simClock = simClock;
        _options = options;
        _timeProvider = timeProvider;
        _observers = observers;
    }

    public async ValueTask ExecuteAsync(ExecutionPlan<TState> plan, SimExecContext context, CancellationToken cancellationToken)
    {
        for (var waveIndex = 0; waveIndex < plan.Waves.Count; waveIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await ExecuteWaveAsync(plan.Waves[waveIndex], waveIndex, context, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async ValueTask ExecuteWaveAsync(ExecutionWave<TState> wave, int waveIndex, SimExecContext context, CancellationToken cancellationToken)
    {
        if (wave.Systems.Count == 0)
            return;

        if (wave.ExecutionMode == SimExecMode.Sequential || _options.MaxDegreeOfParallelism == 1 || wave.Systems.Count == 1)
        {
            await ExecuteSequentiallyAsync(wave.Systems, waveIndex, context, cancellationToken)
                .ConfigureAwait(false);

            return;
        }

        var executions = new SystemExecution[wave.Systems.Count];
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(Enumerable.Range(0, wave.Systems.Count), options, ExecuteAndCollect)
                      .ConfigureAwait(false);

        foreach (var execution in executions)
            execution.Commands.Apply(_state);
        
        return;

        async ValueTask ExecuteAndCollect(int index, CancellationToken token)
        {
            executions[index] = await ExecuteSystemAsync(
                wave.Systems[index],
                waveIndex,
                context,
                token);
        }
    }

    private async ValueTask ExecuteSequentiallyAsync(IReadOnlyList<ISimulationSystem<TState>> systems, int waveIndex, SimExecContext context, CancellationToken cancellationToken)
    {
        foreach (var system in systems)
        {
            var execution = await ExecuteSystemAsync(system, waveIndex, context, cancellationToken).ConfigureAwait(false);

            execution.Commands.Apply(_state);
        }
    }

    private async ValueTask ExecuteInParallelAsync(IReadOnlyList<ISimulationSystem<TState>> systems, int waveIndex, SimExecContext context, CancellationToken cancellationToken)
    {
        var executions = new SystemExecution[systems.Count];

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(Enumerable.Range(0, systems.Count), options, Execute).ConfigureAwait(false);

        foreach (var execution in executions)
            execution.Commands.Apply(_state);
       
        return;

        async ValueTask Execute(int index, CancellationToken token) => executions[index] = await ExecuteSystemAsync(systems[index], waveIndex, context, token);
    }

    private async ValueTask<SystemExecution> ExecuteSystemAsync(ISimulationSystem<TState> system, int waveIndex, SimExecContext context, CancellationToken cancellationToken)
    {
        var commands = new CommandBuffer<TState>();
        var simulationContext = new SimContext<TState>(_state, context.Tick, _simClock, context.Delta, context.Timestamp, commands);
        var startedAt = _timeProvider.GetTimestamp();

        await system.ExecuteAsync(simulationContext, cancellationToken).ConfigureAwait(false);

        var metrics = new SystemMetrics(system.GetType().Name, context.Tick, _timeProvider.GetElapsedTime(startedAt), waveIndex);

        foreach (var observer in _observers)
            observer.OnSystemCompleted(metrics);

        return new SystemExecution(commands);
    }

    private sealed record SystemExecution(CommandBuffer<TState> Commands);
}