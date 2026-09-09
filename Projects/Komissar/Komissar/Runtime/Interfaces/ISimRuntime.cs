using Komissar.Core.Enums;
using Komissar.Utilities;

namespace Komissar.Runtime.Interfaces;

public interface ISimRuntime : IAsyncDisposable
{
    public SimMode Mode { get; }
    public SimStatus Status { get; }

    public long Tick { get; }

    public SimClock SimClock { get; }
    public TimeScale TimeScale { get; }

    public Task StartAsync(CancellationToken cancellationToken = default);
    public Task PauseAsync(CancellationToken cancellationToken = default);
    public Task StopAsync(CancellationToken cancellationToken = default);

    public ValueTask StepAsync(CancellationToken cancellationToken = default);
    public ValueTask StepAsync(TimeSpan delta, CancellationToken cancellationToken = default);
    public ValueTask UpdateAsync(TimeSpan realDelta, CancellationToken cancellationToken = default);
}
