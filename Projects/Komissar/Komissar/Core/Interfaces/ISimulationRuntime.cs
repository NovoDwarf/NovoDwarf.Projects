using Komissar.Core.Enums;
using Komissar.Time;

namespace Komissar.Core.Interfaces;

public interface ISimulationRuntime : IAsyncDisposable
{
    public SimulationMode Mode { get; }
    public SimulationStatus Status { get; }

    public long Tick { get; }

    public Clock Clock { get; }
    public TimeScale TimeScale { get; }

    public Task StartAsync(CancellationToken cancellationToken = default);
    public Task PauseAsync(CancellationToken cancellationToken = default);
    public Task StopAsync(CancellationToken cancellationToken = default);

    public ValueTask StepAsync(CancellationToken cancellationToken = default);
    public ValueTask StepAsync(TimeSpan delta, CancellationToken cancellationToken = default);
    public ValueTask UpdateAsync(TimeSpan realDelta, CancellationToken cancellationToken = default);
}
