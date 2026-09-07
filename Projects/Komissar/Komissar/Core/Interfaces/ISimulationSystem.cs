using Komissar.Executions;

namespace Komissar.Core.Interfaces;

public interface ISimulationSystem<TState>
{
    public SystemDescriptor Descriptor => SystemDescriptor.Exclusive;

    public ValueTask ExecuteAsync(SimulationContext<TState> context, CancellationToken cancellationToken);
}
