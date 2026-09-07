using Komissar.Diagnostics;

namespace Komissar.Core.Interfaces;

public interface ISimulationObserver<TState>
{
    public void OnSystemCompleted(SystemMetrics metrics);

    public void OnTickCompleted(TickMetrics metrics);
}
